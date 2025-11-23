using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.Helpers;
using RecipeApp.Domain.Entities;

namespace RecipeApp.Application.Interfaces;

public interface IRecipeService
{
    Task<Result<RecipeResponse>> AddRecipe(RecipeAddRequest? recipeAddRequest);
    Task<Result<RecipeResponse>> GetRecipeByID(Guid? recipeID);
    Task<Result<List<RecipeResponse>>> GetAllRecipes();
    Task<Result<List<RecipeResponse>>> GetFilteredRecipes(string seachBy, string? seachString);
    Task<Result<List<RecipeResponse>>> GetSortedRecipes(List<RecipeResponse?> recipeList, string sortBy, bool ascending = true);
    Task<Result<RecipeResponse>> UpdateRecipe(RecipeUpdateRequest recipeUpdateRequest);
    Task<Result> DeleteRecipe(Guid? recipeID);
    Task<Result<Recipe>> GetRecipeEntityByID(Guid? recipeID);
}
