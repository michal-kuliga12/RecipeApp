using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.Helpers;

namespace RecipeApp.Application.Interfaces.RecipeInterfaces;

public interface IRecipeQueryService
{
    Task<Result<RecipeResponse>> GetRecipeByID(Guid? recipeID);
    Task<Result<List<RecipeResponse>>> GetAllRecipes();
}
