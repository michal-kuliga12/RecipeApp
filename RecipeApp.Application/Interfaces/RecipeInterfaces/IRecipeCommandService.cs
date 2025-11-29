using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.Helpers;

namespace RecipeApp.Application.Interfaces.RecipeInterfaces;

public interface IRecipeCommandService
{
    Task<Result<RecipeResponse>> AddRecipe(RecipeAddRequest? recipeAddRequest);
    Task<Result<RecipeResponse>> UpdateRecipe(RecipeUpdateRequest recipeUpdateRequest);
}
