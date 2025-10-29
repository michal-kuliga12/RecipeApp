using RecipeApp.Application.DTOs.IngredientDTO;

namespace RecipeApp.Application.Interfaces;

public interface IIngredientService
{
    Task<IngredientResponse?> AddIngredient(IngredientAddRequest ingredientAddRequest);
    Task<IngredientResponse?> GetIngredientByID(Guid? ingredientID);
    Task<List<IngredientResponse?>> GetAllIngredients();
    Task<List<IngredientResponse?>> GetFilteredIngredients(string? searchString);
    Task<IngredientResponse?> UpdateIngredient(IngredientUpdateRequest ingredientUpdateRequest);
    Task<bool> DeleteIngredient(Guid? ingredientID);
}
