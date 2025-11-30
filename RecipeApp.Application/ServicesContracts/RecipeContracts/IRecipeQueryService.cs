using RecipeApp.Core.DTOs.RecipeDTO;
using RecipeApp.Core.Helpers;

namespace RecipeApp.Core.ServicesContracts.RecipeContracts;

public interface IRecipeQueryService
{
    Task<Result<RecipeResponse>> GetRecipeByID(Guid? recipeID);
    Task<Result<List<RecipeResponse>>> GetAllRecipes();
}
