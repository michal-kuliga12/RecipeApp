using RecipeApp.Core.DTOs.RecipeDTO;
using RecipeApp.Core.DTOs.RecipeIngredientDTO;
using RecipeApp.Core.Helpers;

namespace RecipeApp.Core.ServicesContracts;
public interface IRecipeIngredientService
{
    Task<Result<RecipeResponse>> AddRecipeIngredient(RecipeIngredientAddRequest recipeIngredientAddRequest);
    Task<Result<RecipeResponse>> UpdateRecipeIngredient(RecipeIngredientUpdateRequest recipeIngredientUpdateRequest);
    Task<Result> DeleteRecipeIngredient(Guid? recipeIngredientID);
}
