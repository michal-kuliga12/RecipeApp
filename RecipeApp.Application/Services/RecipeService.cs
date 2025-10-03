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
                filteredRecipes = allRecipes.Where(temp => !String.IsNullOrEmpty(temp.Name)
                    && temp.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
                break;
            case nameof(RecipeResponse.Description):
                filteredRecipes = allRecipes.Where(temp => !String.IsNullOrEmpty(temp.Description)
                    && temp.Description.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
                break;
            case nameof(RecipeResponse.Author):
                filteredRecipes = allRecipes.Where(temp => !String.IsNullOrEmpty(temp.Author)
                    && temp.Author.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
                break;
            case nameof(RecipeResponse.Category):
                filteredRecipes = allRecipes.Where(temp => temp.Category != null
                    && EnumDisplayHelper.GetDisplayName(temp.Category).Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
                break;
            case nameof(RecipeResponse.PreparationTime):
                filteredRecipes = allRecipes.Where(temp => temp.PreparationTime != null && temp.PreparationTime >= 1
                    && temp.PreparationTime.ToString().Equals(searchString)).ToList();
                break;
            case nameof(RecipeResponse.RecipeIngredients):
                filteredRecipes = allRecipes.Where(temp => temp.RecipeIngredients != null
                    && temp.RecipeIngredients.Any(ing => ing.Ingredient.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))).ToList();
                break;
            case nameof(RecipeResponse.Servings):
                filteredRecipes = allRecipes.Where(temp => temp.Servings != null && temp.Servings >= 1
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
    public List<RecipeResponse>? GetSortedRecipes(List<RecipeResponse> recipeList, string sortBy, bool ascending = true)
    {
        if (recipeList == null)
            throw new ArgumentNullException("Lista przepisów do sortowania nie może być null");

        if (String.IsNullOrEmpty(sortBy))
            return recipeList;

        List<RecipeResponse> sortedRecipes = (sortBy, ascending) switch
        {
            (nameof(RecipeResponse.Name), ascending: true) => recipeList.OrderBy(temp => temp.Name, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Name), ascending: false) => recipeList.OrderByDescending(temp => temp.Name, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Description), ascending: true) => recipeList.OrderBy(temp => temp.Description, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Description), ascending: false) => recipeList.OrderByDescending(temp => temp.Description, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Author), ascending: true) => recipeList.OrderBy(temp => temp.Author, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Author), ascending: false) => recipeList.OrderByDescending(temp => temp.Author, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Category), ascending: true) => recipeList.OrderBy(temp => temp.Category).ToList(),
            (nameof(RecipeResponse.Category), ascending: false) => recipeList.OrderByDescending(temp => temp.Category).ToList(),
            (nameof(RecipeResponse.PreparationTime), ascending: true) => recipeList.OrderBy(temp => temp.PreparationTime).ToList(),
            (nameof(RecipeResponse.PreparationTime), ascending: false) => recipeList.OrderByDescending(temp => temp.PreparationTime).ToList(),
            (nameof(RecipeResponse.Servings), ascending: true) => recipeList.OrderBy(temp => temp.Servings).ToList(),
            (nameof(RecipeResponse.Servings), ascending: false) => recipeList.OrderByDescending(temp => temp.Servings).ToList(),
            (nameof(RecipeResponse.Rating), ascending: true) => recipeList.OrderBy(temp => temp.Rating).ToList(),
            (nameof(RecipeResponse.Rating), ascending: false) => recipeList.OrderByDescending(temp => temp.Rating).ToList(),
            (nameof(RecipeResponse.CreatedAt), ascending: true) => recipeList.OrderBy(temp => temp.CreatedAt).ToList(),
            (nameof(RecipeResponse.CreatedAt), ascending: false) => recipeList.OrderByDescending(temp => temp.CreatedAt).ToList(),
            _ => recipeList
        };

        return sortedRecipes;
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
