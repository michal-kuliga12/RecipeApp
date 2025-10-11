using RecipeApp.Application.DTOs.IngredientDTO;
using RecipeApp.Application.Interfaces;

namespace RecipeApp.Application.Services
{
    public class IngredientService : IIngredientService
    {
        public IngredientService()
        {
        }

        public IngredientResponse? AddIngredient(IngredientAddRequest ingredientAddRequest)
        {
            throw new NotImplementedException();
        }

        public bool DeleteIngredient(Guid? IngredientID)
        {
            throw new NotImplementedException();
        }

        public List<IngredientResponse>? GetAllIngredients()
        {
            throw new NotImplementedException();
        }

        public List<IngredientResponse>? GetFilteredIngredients(string? searchString)
        {
            throw new NotImplementedException();
        }

        public IngredientResponse? GetIngredientByID(Guid? IngredientID)
        {
            throw new NotImplementedException();
        }

        public IngredientResponse? UpdateIngredient(IngredientUpdateRequest ingredientUpdateRequest)
        {
            throw new NotImplementedException();
        }
    }
}
