using RecipeApp.Application.DTOs.IngredientDTO;
using RecipeApp.Application.Helpers;
using RecipeApp.Application.Interfaces;
using RecipeApp.Domain.Entities;

namespace RecipeApp.Application.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly List<Ingredient> _ingredients;
        public IngredientService(bool initialize = true)
        {
            _ingredients = new List<Ingredient>() { };

            if (initialize = true && !_ingredients.Any())
            {
                AddIngredient(new IngredientAddRequest { Name = "Pomidor" });
                AddIngredient(new IngredientAddRequest { Name = "Ogórek" });
                AddIngredient(new IngredientAddRequest { Name = "Makaron" });
            }
        }

        public IngredientResponse? AddIngredient(IngredientAddRequest ingredientAddRequest)
        {
            if (ingredientAddRequest == null)
                throw new ArgumentNullException("IngredientAddRequest nie może być null");

            bool isValid = ValidationHelper.ValidateModel(ingredientAddRequest);

            if (isValid is false)
                throw new ArgumentException("Niepoprawne wartości");

            Ingredient ingredientToAdd = ingredientAddRequest.ToIngredient();
            ingredientToAdd.ID = Guid.NewGuid();

            _ingredients.Add(ingredientToAdd);

            return ingredientToAdd.ToIngredientResponse();
        }

        public bool DeleteIngredient(Guid? ingredientID)
        {
            if (ingredientID is null)
                throw new ArgumentNullException("IngredientID nie może być null");

            Ingredient? ingredientToDelete = _ingredients.SingleOrDefault(i => i.ID == ingredientID);

            if (ingredientToDelete is null)
                return false;

            return true;
        }

        public List<IngredientResponse>? GetAllIngredients()
            => _ingredients.Select(i => i.ToIngredientResponse()).ToList();

        public List<IngredientResponse>? GetFilteredIngredients(string? searchString)
        {
            if (searchString is null || searchString == String.Empty)
                return _ingredients.Select(i => i.ToIngredientResponse()).ToList();

            return _ingredients
                .Where(i => i.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                .Select(i => i.ToIngredientResponse())
                .ToList();
        }

        public IngredientResponse? GetIngredientByID(Guid? ingredientID)
        {
            if (ingredientID is null)
                throw new ArgumentNullException("IngredientID nie może być null");

            Ingredient? ingredientFound = _ingredients.SingleOrDefault(i => i.ID == ingredientID);

            if (ingredientFound is null)
                return null;

            return ingredientFound.ToIngredientResponse();
        }

        public IngredientResponse? UpdateIngredient(IngredientUpdateRequest ingredientUpdateRequest)
        {
            if (ingredientUpdateRequest is null || ingredientUpdateRequest.ID is null)
                throw new ArgumentNullException(nameof(ingredientUpdateRequest), "ingredientUpdateRequest nie może być null");

            bool isValid = ValidationHelper.ValidateModel(ingredientUpdateRequest);

            if (!isValid)
                throw new ArgumentException($"Obiekt ingredientUpdateRequest jest niepoprawny");

            Ingredient? ingredientFound = _ingredients.SingleOrDefault(i => i.ID == ingredientUpdateRequest.ID);

            if (ingredientFound is null)
                throw new ArgumentException($"Nie znaleziono składnika o ID:{ingredientUpdateRequest.ID}");

            ingredientFound.Name = ingredientUpdateRequest.Name;
            return ingredientFound.ToIngredientResponse();
        }
    }
}
