using System.Linq.Expressions;
using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.Helpers;
using RecipeApp.Application.Interfaces;
using RecipeApp.Domain.Entities;
using RecipeApp.Infrastructure.Repositories;

namespace RecipeApp.Application.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeService(IRecipeRepository recipeRepository)
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

    public async Task<Result<RecipeResponse>> GetRecipeByID(Guid? inID)
    {
        if (inID is null)
            return Result<RecipeResponse>.Failure("Nie podano ID");

        Recipe? recipeFound = await _recipeRepository.GetRecipeByID(inID.Value);

        if (recipeFound is null)
            return Result<RecipeResponse>.Failure("Nie znaleziono szukanego przepisu.");

        return Result<RecipeResponse>.Success(recipeFound.ToRecipeResponse());
    }

    public async Task<Result<List<RecipeResponse>>> GetAllRecipes()
    {
        var recipes = await _recipeRepository.GetAllRecipes();
        List<RecipeResponse> recipesAsResponse = recipes.Select(temp => temp.ToRecipeResponse()).ToList();
        return Result<List<RecipeResponse>>.Success(recipesAsResponse);
    }

    public async Task<Result<List<RecipeResponse>>> GetFilteredRecipes(string searchBy, string? searchString)
    {
        List<Recipe> recipes = new List<Recipe>();

        if (string.IsNullOrWhiteSpace(searchBy) || string.IsNullOrWhiteSpace(searchString))
        {
            var all = await _recipeRepository.GetAllRecipes();
            return Result<List<RecipeResponse>>.Success(all.Select(r => r.ToRecipeResponse()).ToList());
        }

        Expression<Func<Recipe, bool>> filter = ApplyFilter(searchBy, searchString);

        var filtered = await _recipeRepository.GetFilteredRecipes(filter)
            ?? new List<Recipe>();

        return Result<List<RecipeResponse>>.Success(filtered.Select(r => r.ToRecipeResponse()).ToList());
    }

    public async Task<Result<List<RecipeResponse>>> GetSortedRecipes(List<RecipeResponse> inRecipes, string sortBy, bool ascending = true)
    {
        if (inRecipes is null)
            return Result<List<RecipeResponse>>.Failure("Lista przepisów do sortowania nie może być null");

        if (String.IsNullOrEmpty(sortBy))
            return Result<List<RecipeResponse>>.Success(inRecipes);

        List<RecipeResponse> sorted = (sortBy, ascending) switch
        {
            (nameof(RecipeResponse.Name), ascending: true) => inRecipes.OrderBy(temp => temp.Name, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Name), ascending: false) => inRecipes.OrderByDescending(temp => temp.Name, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Description), ascending: true) => inRecipes.OrderBy(temp => temp.Description, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Description), ascending: false) => inRecipes.OrderByDescending(temp => temp.Description, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Author), ascending: true) => inRecipes.OrderBy(temp => temp.Author, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Author), ascending: false) => inRecipes.OrderByDescending(temp => temp.Author, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Category), ascending: true) => inRecipes.OrderBy(temp => temp.Category).ToList(),
            (nameof(RecipeResponse.Category), ascending: false) => inRecipes.OrderByDescending(temp => temp.Category).ToList(),
            (nameof(RecipeResponse.PreparationTime), ascending: true) => inRecipes.OrderBy(temp => temp.PreparationTime).ToList(),
            (nameof(RecipeResponse.PreparationTime), ascending: false) => inRecipes.OrderByDescending(temp => temp.PreparationTime).ToList(),
            (nameof(RecipeResponse.Servings), ascending: true) => inRecipes.OrderBy(temp => temp.Servings).ToList(),
            (nameof(RecipeResponse.Servings), ascending: false) => inRecipes.OrderByDescending(temp => temp.Servings).ToList(),
            (nameof(RecipeResponse.Rating), ascending: true) => inRecipes.OrderBy(temp => temp.Rating).ToList(),
            (nameof(RecipeResponse.Rating), ascending: false) => inRecipes.OrderByDescending(temp => temp.Rating).ToList(),
            (nameof(RecipeResponse.CreatedAt), ascending: true) => inRecipes.OrderBy(temp => temp.CreatedAt).ToList(),
            (nameof(RecipeResponse.CreatedAt), ascending: false) => inRecipes.OrderByDescending(temp => temp.CreatedAt).ToList(),
            _ => inRecipes
        };

        return Result<List<RecipeResponse>>.Success(sorted);
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
    public async Task<Result> DeleteRecipe(Guid? inID)
    {
        if (inID is null)
            throw new ArgumentNullException("RecipeID nie może być null");

        Recipe? recipeToDelete = await _recipeRepository.GetRecipeByID(inID.Value);

        if (recipeToDelete is null)
            return Result.Failure("Nie znaleziono przepisu w bazie danych");

        await _recipeRepository.DeleteRecipe(inID.Value);

        return Result.Success();
    }

    public async Task<Result<Recipe>> GetRecipeEntityByID(Guid? inID)
    {
        if (inID is null)
            throw new ArgumentNullException("RecipeID nie może być null");
        Recipe? dbRecipe = await _recipeRepository.GetRecipeByID(inID.Value);

        if (dbRecipe is null)
            return Result<Recipe>.Failure("Nie znaleziono przepisu w bazie danych");

        return Result<Recipe>.Success(dbRecipe);
    }


    #region Helper methods
    private Expression<Func<Recipe, bool>> ApplyFilter(string searchBy, string? searchString)
    {
        return searchBy switch
        {
            nameof(RecipeResponse.Name) =>
                temp => !string.IsNullOrEmpty(temp.Name)
                        && temp.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase),

            nameof(RecipeResponse.Description) =>
                temp => !string.IsNullOrEmpty(temp.Description)
                        && temp.Description.Contains(searchString, StringComparison.CurrentCultureIgnoreCase),

            nameof(RecipeResponse.Author) =>
                temp => !string.IsNullOrEmpty(temp.Author)
                        && temp.Author.Contains(searchString, StringComparison.CurrentCultureIgnoreCase),

            nameof(RecipeResponse.Category) =>
                temp => temp.Category != null
                        && EnumDisplayHelper.GetDisplayName(temp.Category).Contains(searchString, StringComparison.CurrentCultureIgnoreCase),

            nameof(RecipeResponse.PreparationTime) =>
                temp => temp.PreparationTime != null
                        && temp.PreparationTime >= 1
                        && temp.PreparationTime.ToString() == searchString,

            nameof(RecipeResponse.RecipeIngredients) =>
                temp => temp.RecipeIngredients != null
                        && temp.RecipeIngredients.Any
                            (
                                ing => ing.Ingredient.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                            ),
            nameof(RecipeResponse.Servings) =>
                temp => temp.Servings != null
                        && temp.Servings >= 1
                        && temp.Servings.ToString() == searchString,

            nameof(RecipeResponse.Rating) =>
                temp => temp.Rating != null
                        && temp.Rating.ToString() == searchString,

            _ => temp => true
        };
    }
    #endregion
}
