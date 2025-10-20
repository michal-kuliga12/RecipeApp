using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.DTOs.RecipeIngredientDTO;
using RecipeApp.Domain.Entities;

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
    Recipe? GetRecipeEntityByID(Guid? recipeID);
}
