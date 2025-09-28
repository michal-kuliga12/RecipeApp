using RecipeApp.Application.DTOs;

namespace RecipeApp.Application.Interfaces;

public interface IRecipeService
{
    RecipeResponse? AddRecipe(RecipeAddRequest? recipeAddRequest);
    RecipeResponse? GetRecipeByID(Guid? recipeID);
    List<RecipeResponse>? GetFilteredRecipes(string searchBy, string? searchString);
    List<RecipeResponse>? GetAllRecipes();
    List<RecipeResponse>? UpdateRecipe(RecipeUpdateRequest recipeUpdateRequest);
    bool DeleteRecipe(Guid? recipeID);
}
