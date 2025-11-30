using RecipeApp.Core.DTOs.IngredientDTO;
using RecipeApp.Core.Helpers;

namespace RecipeApp.Core.ServicesContracts;

public interface IIngredientService
{
    Task<Result<IngredientResponse>> AddIngredient(IngredientAddRequest ingredientAddRequest);
    Task<Result<IngredientResponse>> GetIngredientByID(Guid? ingredientID);
    Task<Result<List<IngredientResponse>>> GetAllIngredients();
    Task<Result<List<IngredientResponse>>> GetFilteredIngredients(string? searchString);
    Task<Result<IngredientResponse>> UpdateIngredient(IngredientUpdateRequest ingredientUpdateRequest);
    Task<Result> DeleteIngredient(Guid? ingredientID);
}
