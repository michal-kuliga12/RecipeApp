using RecipeApp.Core.DTOs.RecipeDTO;
using RecipeApp.Core.Helpers;

namespace RecipeApp.Core.ServicesContracts.RecipeContracts;

public interface IRecipeFilterService
{
    Task<Result<List<RecipeResponse>>> GetFilteredRecipes(string seachBy, string? seachString);
    Task<Result<List<RecipeResponse>>> GetSortedRecipes(List<RecipeResponse?> recipeList, string sortBy, bool ascending = true);
}
