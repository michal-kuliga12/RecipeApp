using Entities;
using RecipeApp.Application.DTOs;
using RecipeApp.Application.Helpers;
using RecipeApp.Application.Interfaces;

namespace RecipeApp.Application.Services;

public class RecipeService : IRecipeService
{
    private readonly List<Recipe> _recipes;

    public RecipeService()
    {
        _recipes = new List<Recipe>();
    }

    public RecipeResponse AddRecipe(RecipeAddRequest? recipeAddRequest)
    {
        if (recipeAddRequest is null)
            throw new ArgumentNullException("RecipeAddRequest nie może być null");

        bool isModelValid = ValidationHelper.ValidateModel(recipeAddRequest);

        if (!isModelValid)
            throw new ArgumentNullException("Model jest niepoprawny");

        Recipe recipe = recipeAddRequest.ToRecipe();
        recipe.ID = Guid.NewGuid();
        _recipes.Add(recipe);
        RecipeResponse recipeResponse = recipe.ToRecipeResponse();

        return recipeResponse;

    }
    public RecipeResponse? GetRecipeByID(Guid? recipeID)
    {
        if (recipeID == null)
            return null;

        Recipe? recipeFound = _recipes.SingleOrDefault(temp => temp.ID == recipeID);

        if (recipeFound == null)
            return null;

        return recipeFound.ToRecipeResponse();
    }

    public List<RecipeResponse> GetAllRecipes()
        => _recipes.Select(temp => temp.ToRecipeResponse()).ToList();

    public List<RecipeResponse> GetFilteredRecipes(string searchBy, string? searchString)
    {
        throw new NotImplementedException();
    }

    public List<RecipeResponse> UpdateRecipe(RecipeUpdateRequest recipeUpdateRequest)
    {
        throw new NotImplementedException();
    }

    public bool DeleteRecipe(Guid? recipeID)
    {
        throw new NotImplementedException();
    }
}
