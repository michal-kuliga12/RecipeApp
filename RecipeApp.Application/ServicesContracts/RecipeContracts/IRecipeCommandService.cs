using RecipeApp.Core.DTOs.RecipeDTO;
using RecipeApp.Core.Helpers;

namespace RecipeApp.Core.ServicesContracts.RecipeContracts;

public interface IRecipeCommandService
{
    Task<Result<RecipeResponse>> AddRecipe(RecipeAddRequest? recipeAddRequest);
    Task<Result<RecipeResponse>> UpdateRecipe(RecipeUpdateRequest recipeUpdateRequest);
}
