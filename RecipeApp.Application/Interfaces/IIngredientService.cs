using RecipeApp.Application.DTOs.IngredientDTO;
using RecipeApp.Application.Helpers;

namespace RecipeApp.Application.Interfaces;

public interface IIngredientService
{
    Task<Result<IngredientResponse>> AddIngredient(IngredientAddRequest ingredientAddRequest);
    Task<Result<IngredientResponse>> GetIngredientByID(Guid? ingredientID);
    Task<Result<List<IngredientResponse>>> GetAllIngredients();
    Task<Result<List<IngredientResponse>>> GetFilteredIngredients(string? searchString);
    Task<Result<IngredientResponse>> UpdateIngredient(IngredientUpdateRequest ingredientUpdateRequest);
    Task<Result> DeleteIngredient(Guid? ingredientID);
}
