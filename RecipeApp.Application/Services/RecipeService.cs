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
        List<RecipeResponse> allRecipes = GetAllRecipes();
        List<RecipeResponse> filteredRecipes = allRecipes;

        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            return filteredRecipes;

        switch (searchBy)
        {
            case nameof(RecipeResponse.Name):
                filteredRecipes = allRecipes.Where(temp => temp.Name != null
                    && temp.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
                break;
            case nameof(RecipeResponse.Description):
                filteredRecipes = allRecipes.Where(temp => temp.Description != null
                    && temp.Description.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
                break;
            case nameof(RecipeResponse.Author):
                filteredRecipes = allRecipes.Where(temp => temp.Author != null
                    && temp.Author.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
                break;
            case nameof(RecipeResponse.Category):
                filteredRecipes = allRecipes.Where(temp => temp.Category != null
                    && EnumDisplayHelper.GetDisplayName(temp.Category).Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
                break;
            case nameof(RecipeResponse.PreparationTime):
                filteredRecipes = allRecipes.Where(temp => temp.PreparationTime != null
                    && temp.PreparationTime.ToString().Equals(searchString)).ToList();
                break;
            case nameof(RecipeResponse.RecipeIngredients):
                filteredRecipes = allRecipes.Where(temp => temp.RecipeIngredients != null
                    && temp.RecipeIngredients.Any(ing => ing.Ingredient.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))).ToList();
                break;
            case nameof(RecipeResponse.Servings):
                filteredRecipes = allRecipes.Where(temp => temp.Servings != null
                    && temp.Servings.ToString().Equals(searchString)).ToList();
                break;
            case nameof(RecipeResponse.Rating):
                filteredRecipes = allRecipes.Where(temp => temp.Rating != null
                    && temp.Rating.ToString().Equals(searchString)).ToList();
                break;
            default:
                filteredRecipes = allRecipes;
                break;
        }
        return filteredRecipes;
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
