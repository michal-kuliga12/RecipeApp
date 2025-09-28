using RecipeApp.Application.DTOs;

namespace RecipeApp.Application.Interfaces;

public interface IRecipeService
{
    RecipeResponse AddRecipe(RecipeAddRequest? recipeAddRequest);
    List<RecipeResponse> GetAllRecipes();
    RecipeResponse GetRecipeByID(Guid? recipeID);
}
