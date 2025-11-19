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

    public async Task<RecipeResponse?> AddRecipe(RecipeAddRequest? recipeAddRequest)
    {
        if (recipeAddRequest is null)
            throw new ArgumentNullException("RecipeAddRequest nie może być null");

        bool isModelValid = ValidationHelper.ValidateModel(recipeAddRequest);

        if (!isModelValid)
            throw new ArgumentNullException("Model jest niepoprawny");

        Recipe recipe = recipeAddRequest.ToRecipe();
        recipe.ID = Guid.NewGuid();

        await _recipeRepository.AddRecipe(recipe);

        RecipeResponse recipeResponse = recipe.ToRecipeResponse();

        return recipeResponse;

    }
    public async Task<RecipeResponse?> GetRecipeByID(Guid? recipeID)
    {
        if (recipeID == null)
            return null;

        Recipe? recipeFound = await _recipeRepository.GetRecipeByID(recipeID.Value);

        if (recipeFound == null)
            return null;

        return recipeFound.ToRecipeResponse();
    }
    public async Task<List<RecipeResponse>?> GetAllRecipes()
    {
        var recipes = await _recipeRepository.GetAllRecipes();
        return recipes.Select(temp => temp.ToRecipeResponse()).ToList();
    }
    public async Task<List<RecipeResponse>?> GetFilteredRecipes(string searchBy, string? searchString)
    {
        List<Recipe> recipes = new List<Recipe>();

        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            return (await _recipeRepository.GetAllRecipes()).Select(temp => temp.ToRecipeResponse()).ToList();

        recipes = searchBy switch
        {
            nameof(RecipeResponse.Name) =>
                await _recipeRepository.GetFilteredRecipes(temp =>
                    !string.IsNullOrEmpty(temp.Name)
                    && temp.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
                ?? new List<Recipe>(),

            nameof(RecipeResponse.Description) =>
                await _recipeRepository.GetFilteredRecipes(temp =>
                    !string.IsNullOrEmpty(temp.Description)
                    && temp.Description.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
                ?? new List<Recipe>(),

            nameof(RecipeResponse.Author) =>
                await _recipeRepository.GetFilteredRecipes(temp =>
                    !string.IsNullOrEmpty(temp.Author)
                    && temp.Author.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
                ?? new List<Recipe>(),

            nameof(RecipeResponse.Category) =>
                await _recipeRepository.GetFilteredRecipes(temp =>
                    temp.Category != null
                    && EnumDisplayHelper.GetDisplayName(temp.Category)
                        .Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
                ?? new List<Recipe>(),

            nameof(RecipeResponse.PreparationTime) =>
                await _recipeRepository.GetFilteredRecipes(temp =>
                    temp.PreparationTime != null
                    && temp.PreparationTime >= 1
                    && temp.PreparationTime.ToString() == searchString)
                ?? new List<Recipe>(),

            nameof(RecipeResponse.RecipeIngredients) =>
                await _recipeRepository.GetFilteredRecipes(temp =>
                    temp.RecipeIngredients != null
                    && temp.RecipeIngredients.Any(ing =>
                        ing.Ingredient.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                ?? new List<Recipe>(),

            nameof(RecipeResponse.Servings) =>
                await _recipeRepository.GetFilteredRecipes(temp =>
                    temp.Servings != null
                    && temp.Servings >= 1
                    && temp.Servings.ToString() == searchString)
                ?? new List<Recipe>(),

            nameof(RecipeResponse.Rating) =>
                await _recipeRepository.GetFilteredRecipes(temp =>
                    temp.Rating != null
                    && temp.Rating.ToString() == searchString)
                ?? new List<Recipe>(),

            _ => (await _recipeRepository.GetAllRecipes()) ?? new List<Recipe>()
        };

        return recipes.Select(temp => temp.ToRecipeResponse()).ToList();
    }
    public async Task<List<RecipeResponse>?> GetSortedRecipes(List<RecipeResponse> recipeList, string sortBy, bool ascending = true)
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
    public async Task<RecipeResponse?> UpdateRecipe(RecipeUpdateRequest recipeUpdateRequest)
    {
        if (recipeUpdateRequest is null)
            throw new ArgumentNullException("RecipeUpdateRequest nie może być null");

        bool isModelValid = ValidationHelper.ValidateModel(recipeUpdateRequest);

        if (!isModelValid)
            throw new ArgumentNullException("Model jest niepoprawny");


        Recipe? recipeFound = await _recipeRepository.GetRecipeByID(recipeUpdateRequest.ID.Value);

        if (recipeFound is null)
            return null;

        recipeFound.Name = recipeUpdateRequest.Name;
        recipeFound.Description = recipeUpdateRequest.Description;
        recipeFound.Author = recipeUpdateRequest.Author;
        recipeFound.Category = recipeUpdateRequest.Category;
        recipeFound.PreparationTime = recipeUpdateRequest.PreparationTime;
        recipeFound.RecipeIngredients = recipeUpdateRequest.RecipeIngredients;
        recipeFound.Servings = recipeUpdateRequest.Servings;
        recipeFound.Rating = recipeUpdateRequest.Rating;
        recipeFound.ImageUrl = recipeUpdateRequest.ImageUrl;

        await _recipeRepository.UpdateRecipe(recipeFound);

        return recipeFound.ToRecipeResponse();
    }
    public async Task<bool> DeleteRecipe(Guid? recipeID)
    {
        if (recipeID is null)
            throw new ArgumentNullException("RecipeID nie może być null");

        Recipe? recipeToDelete = await _recipeRepository.GetRecipeByID(recipeID.Value);

        if (recipeToDelete == null)
            return false;

        await _recipeRepository.DeleteRecipe(recipeID.Value);

        return true;
    }
    public async Task<Recipe?> GetRecipeEntityByID(Guid? recipeID)
    {
        if (recipeID == null)
            return null;
        Recipe? recipeFound = await _recipeRepository.GetRecipeByID(recipeID.Value);

        if (recipeFound is null)
            return null;

        return recipeFound;
    }
}
