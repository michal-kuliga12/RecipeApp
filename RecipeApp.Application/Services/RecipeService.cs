using RecipeApp.Application.DTOs;
using RecipeApp.Application.Interfaces;

namespace RecipeApp.Application.Services;

public class RecipeService : IRecipeService
{
    public RecipeResponse AddRecipe(RecipeAddRequest? recipeAddRequest)
    {
        throw new NotImplementedException();
    }

    public List<RecipeResponse> GetAllRecipes()
    {
        throw new NotImplementedException();
    }

    public RecipeResponse GetRecipeByID(Guid? recipeID)
    {
        throw new NotImplementedException();
    }
}
