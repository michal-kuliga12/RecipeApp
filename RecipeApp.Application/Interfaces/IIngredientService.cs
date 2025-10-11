using RecipeApp.Application.DTOs.IngredientDTO;

namespace RecipeApp.Application.Interfaces;

public interface IIngredientService
{
    IngredientResponse? AddIngredient(IngredientAddRequest ingredientAddRequest);
    IngredientResponse? GetIngredientByID(Guid? ingredientID);
    List<IngredientResponse>? GetAllIngredients();
    List<IngredientResponse>? GetFilteredIngredients(string? searchString);
    IngredientResponse? UpdateIngredient(IngredientUpdateRequest ingredientUpdateRequest);
    bool DeleteIngredient(Guid? ingredientID);
}
