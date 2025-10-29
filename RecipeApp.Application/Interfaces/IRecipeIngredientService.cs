using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.DTOs.RecipeIngredientDTO;

namespace RecipeApp.Application.Interfaces;

public interface IRecipeIngredientService
{
    Task<RecipeResponse?> AddRecipeIngredient(RecipeIngredientAddRequest recipeIngredientAddRequest);
    Task<RecipeResponse?> UpdateRecipeIngredient(RecipeIngredientUpdateRequest recipeIngredientUpdateRequest);
    Task<bool> DeleteRecipeIngredient(Guid? recipeIngredientID);
}
