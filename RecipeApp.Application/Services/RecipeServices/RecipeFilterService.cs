using System.Linq.Expressions;
using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.Helpers;
using RecipeApp.Application.Interfaces.RecipeInterfaces;
using RecipeApp.Domain.Entities;
using RecipeApp.Domain.RepositoriesContracts;

namespace RecipeApp.Application.RecipeServices;

public class RecipeFilterService : IRecipeFilterService
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeFilterService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
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
