using RecipeApp.Application.DTOs.RecipeDTO;

namespace RecipeApp.Application.Interfaces;

public interface IRecipeService
{
    RecipeResponse? AddRecipe(RecipeAddRequest? recipeAddRequest);
    RecipeResponse? GetRecipeByID(Guid? recipeID);
    List<RecipeResponse>? GetAllRecipes();
    List<RecipeResponse>? GetFilteredRecipes(string searchBy, string? searchString);
    List<RecipeResponse>? GetSortedRecipes(List<RecipeResponse> recipeList, string sortBy, bool ascending = true);
    RecipeResponse? UpdateRecipe(RecipeUpdateRequest recipeUpdateRequest);
    bool DeleteRecipe(Guid? recipeID);
}
