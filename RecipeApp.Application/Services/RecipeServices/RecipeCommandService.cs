using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.Helpers;
using RecipeApp.Application.Interfaces.RecipeInterfaces;
using RecipeApp.Domain.Entities;
using RecipeApp.Domain.RepositoriesContracts;

namespace RecipeApp.Application.RecipeServices;

public class RecipeCommandService : IRecipeCommandService
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeCommandService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public async Task<Result<RecipeResponse>> AddRecipe(RecipeAddRequest? recipeAddRequest)
    {
        if (recipeAddRequest is null)
            throw new ArgumentNullException("Recipe Add Request nie może być null");

        bool isModelValid = ValidationHelper.ValidateModel(recipeAddRequest);

        if (!isModelValid)
            return Result<RecipeResponse>.Failure("Wprowadzono niepoprawne dane");

        Recipe recipe = recipeAddRequest.ToRecipe();
        recipe.ID = Guid.NewGuid();

        await _recipeRepository.AddRecipe(recipe);

        RecipeResponse recipeResponse = recipe.ToRecipeResponse();

        return Result<RecipeResponse>.Success(recipeResponse);

    }


    public async Task<Result<RecipeResponse>> UpdateRecipe(RecipeUpdateRequest recipeUpdateRequest)
    {
        if (recipeUpdateRequest is null)
            throw new ArgumentNullException("RecipeUpdateRequest nie może być null");

        bool isModelValid = ValidationHelper.ValidateModel(recipeUpdateRequest);

        if (!isModelValid)
            return Result<RecipeResponse>.Failure("Wprowadzono niepoprawne dane");

        Recipe? dbRecipe = await _recipeRepository.GetRecipeByID(recipeUpdateRequest.ID.Value);

        if (dbRecipe is null)
            return Result<RecipeResponse>.Failure("Nie znaleziono przepisu w bazie danych");

        dbRecipe.Name = recipeUpdateRequest.Name;
        dbRecipe.Description = recipeUpdateRequest.Description;
        dbRecipe.Author = recipeUpdateRequest.Author;
        dbRecipe.Category = recipeUpdateRequest.Category;
        dbRecipe.PreparationTime = recipeUpdateRequest.PreparationTime;
        dbRecipe.RecipeIngredients = recipeUpdateRequest.RecipeIngredients;
        dbRecipe.Servings = recipeUpdateRequest.Servings;
        dbRecipe.Rating = recipeUpdateRequest.Rating;
        dbRecipe.ImageUrl = recipeUpdateRequest.ImageUrl;

        await _recipeRepository.UpdateRecipe(dbRecipe);

        return Result<RecipeResponse>.Success(dbRecipe.ToRecipeResponse());
    }

}
