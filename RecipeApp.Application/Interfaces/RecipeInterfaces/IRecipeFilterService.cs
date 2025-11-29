using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.Helpers;

namespace RecipeApp.Application.Interfaces.RecipeInterfaces;

public interface IRecipeFilterService
{
    Task<Result<List<RecipeResponse>>> GetFilteredRecipes(string seachBy, string? seachString);
    Task<Result<List<RecipeResponse>>> GetSortedRecipes(List<RecipeResponse?> recipeList, string sortBy, bool ascending = true);
}
