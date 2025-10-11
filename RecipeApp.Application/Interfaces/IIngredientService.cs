using RecipeApp.Application.DTOs.Ingredient;

namespace RecipeApp.Application.Interfaces;

public interface IIngredientService
{
    IngredientResponse? AddIngredient(IngredientAddRequest ingredientAddRequest);
    IngredientResponse? GetIngredientByID(Guid? IngredientID);
    List<IngredientResponse>? GetAllIngredients();
    List<IngredientResponse>? GetFilteredIngredients(string? searchString);
    IngredientResponse? UpdateIngredient(IngredientUpdateRequest ingredientUpdateRequest);
    bool? DeleteIngredient(Guid? IngredientID);
}
