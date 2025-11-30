using RecipeApp.Application.DTOs.IngredientDTO;
using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.DTOs.RecipeIngredientDTO;
using RecipeApp.Application.Helpers;
using RecipeApp.Application.Interfaces;
using RecipeApp.Domain.Entities;
using RecipeApp.Domain.RepositoriesContracts;

namespace RecipeApp.Application.Services;

public class RecipeIngredientService : IRecipeIngredientService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IIngredientService _ingredientService;

    public RecipeIngredientService(IRecipeRepository recipeRepository, IIngredientRepository ingredientRepository, IIngredientService ingredientService)
    {
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
        _ingredientService = ingredientService;
    }

    public async Task<Result<RecipeResponse>> AddRecipeIngredient(RecipeIngredientAddRequest request)
    {
        if (request is null)
            throw new ArgumentNullException("RecipeIngredientAddRequest nie może być null");

        bool isModelValid = ValidationHelper.ValidateModel(request);
        if (!isModelValid)
            return Result<RecipeResponse>.Failure("Wprowadzono niepoprawne dane");

        var recipeFound = await _recipeRepository.GetRecipeByID(request.RecipeID);
        if (recipeFound is null)
            return Result<RecipeResponse>.Failure("Nie znaleziono przepisu w bazie danych");

        recipeFound.RecipeIngredients ??= new List<RecipeIngredient>();
        bool recipeIngredientExists = recipeFound.RecipeIngredients.Any(ri => ri.IngredientID == request.IngredientID);
        if (recipeIngredientExists)
            throw new InvalidOperationException("Składnik o podanym ID jest już przypisany do przepisu.");

        Task<Result<Guid>> ingIDResult = GetOrCreateIngredientID(request.IngredientName, request.IngredientID);
        if (ingIDResult.Result.IsSuccess)
        {
            request.IngredientID = ingIDResult.Result.Value;
            RecipeIngredient recipeIngredient = request.ToRecipeIngredient();
            recipeIngredient.ID = Guid.NewGuid();

            Recipe updatedRecipe = await _recipeRepository.InsertRecipeIngredient(recipeIngredient);
            if (updatedRecipe is null)
                throw new InvalidOperationException("Recipe nie powinno zwracać null");

            return Result<RecipeResponse>.Success(updatedRecipe.ToRecipeResponse());
        }
        else
        {
            return Result<RecipeResponse>.Failure(ingIDResult.Result.Error);
        }
    }

    private async Task<Result<Guid>> GetOrCreateIngredientID(string? name, Guid id)
    {
        bool ingredientIDExists = id != Guid.Empty;
        if (ingredientIDExists)
        {
            Ingredient? ingredientFromDB = await _ingredientRepository.GetIngredientByID(id);
            if (ingredientFromDB is null)
                return Result<Guid>.Failure("Składnik z podanego IngredientID nie istnieje w bazie danych");
            return Result<Guid>.Success(id);
        }

        bool ingredientNameExists = !String.IsNullOrWhiteSpace(name);
        if (ingredientNameExists)
        {
            List<Ingredient> repoIngredientsWithName = await _ingredientRepository.GetFilteredIngredients(i => i.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            Ingredient? ingredientFromDB = repoIngredientsWithName.SingleOrDefault();

            if (ingredientFromDB != null)
            {
                id = ingredientFromDB.ID;
            }
            else
            {
                IngredientAddRequest ingredientToAdd = new()
                {
                    Name = name,
                };
                Result<IngredientResponse>? ingredientResponseFromAdd = await _ingredientService.AddIngredient(ingredientToAdd);
                id = ingredientResponseFromAdd.Value.ID;
            }
            return Result<Guid>.Success(id);
        }
        throw new InvalidOperationException("IngredientID lub IngredientName musi istnieć");
    }

    public Task<Result> DeleteRecipeIngredient(Guid? recipeIngredientID)
    {
        throw new NotImplementedException();
    }

    public Task<Result<RecipeResponse>> UpdateRecipeIngredient(RecipeIngredientUpdateRequest recipeIngredientUpdateRequest)
    {
        throw new NotImplementedException();
    }
    //public async Task<RecipeResponse>? UpdateRecipeIngredient(RecipeIngredientUpdateRequest recipeIngredientUpdateRequest)
    //{
    //    if (recipeIngredientUpdateRequest is null)
    //        throw new ArgumentNullException("RecipeIngredientUpdateRequest nie może być null");

    //    bool isValid = ValidationHelper.ValidateModel(recipeIngredientUpdateRequest);
    //    if (!isValid)
    //        throw new ArgumentException("Wprowadzono niepoprawne dane");

    //    Guid? ingredientID = recipeIngredientUpdateRequest.IngredientID;
    //    IngredientResponse? ingredientFound = await _ingredientService.GetIngredientByID(ingredientID);
    //    if (ingredientFound is null)
    //        throw new InvalidOperationException("Podany przepis nie istnieje w bazie danych");

    //    Guid? recipeID = recipeIngredientUpdateRequest.RecipeID;
    //    Recipe? recipeFound = await _recipeService.GetRecipeEntityByID(recipeID);
    //    if (recipeFound is null)
    //        throw new InvalidOperationException("Podany przepis nie istnieje w bazie danych");

    //    RecipeIngredient? recipeIngredientToModify = recipeFound.RecipeIngredients.FirstOrDefault(i => i.ID == recipeIngredientUpdateRequest.ID);
    //    if (recipeIngredientToModify is null)
    //        throw new InvalidOperationException("Nie znaleziono przepisu do modyfikacji");

    //    recipeIngredientToModify.Quantity = recipeIngredientUpdateRequest.Quantity.Value!;
    //    recipeIngredientToModify.Unit = recipeIngredientUpdateRequest.Unit.Value!;
    //    await _db.SaveChangesAsync();


    //    return recipeFound.ToRecipeResponse();
    //}

}

