using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.DTOs.RecipeIngredientDTO;

namespace RecipeApp.Application.Interfaces;

public interface IRecipeIngredientService
{
    RecipeResponse? AddRecipeIngredient(RecipeIngredientAddRequest recipeIngredientAddRequest);
    RecipeResponse? UpdateRecipeIngredient(RecipeIngredientUpdateRequest recipeIngredientUpdateRequest);
    bool DeleteRecipeIngredient(Guid? recipeIngredientID);
}
