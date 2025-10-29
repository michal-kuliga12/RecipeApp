using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Domain.Entities;

namespace RecipeApp.Application.Interfaces;

public interface IRecipeService
{
    Task<RecipeResponse?> AddRecipe(RecipeAddRequest? recipeAddRequest);
    Task<RecipeResponse?> GetRecipeByID(Guid? recipeID);
    Task<List<RecipeResponse?>> GetAllRecipes();
    Task<List<RecipeResponse?>> GetFilteredRecipes(string seachBy, string? seachString);
    Task<List<RecipeResponse?>> GetSortedRecipes(List<RecipeResponse?> recipeList, string sortBy, bool ascending = true);
    Task<RecipeResponse?> UpdateRecipe(RecipeUpdateRequest recipeUpdateRequest);
    Task<bool> DeleteRecipe(Guid? recipeID);
    Task<Recipe?> GetRecipeEntityByID(Guid? recipeID);
}
