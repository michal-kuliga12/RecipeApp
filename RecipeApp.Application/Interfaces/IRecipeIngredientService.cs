using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.DTOs.RecipeIngredientDTO;
using RecipeApp.Application.Helpers;

namespace RecipeApp.Application.Interfaces;

public interface IRecipeIngredientService
{
    Task<Result<RecipeResponse>> AddRecipeIngredient(RecipeIngredientAddRequest recipeIngredientAddRequest);
    Task<Result<RecipeResponse>> UpdateRecipeIngredient(RecipeIngredientUpdateRequest recipeIngredientUpdateRequest);
    Task<Result> DeleteRecipeIngredient(Guid? recipeIngredientID);
}
