using Microsoft.EntityFrameworkCore;
using RecipeApp.Application.DTOs.IngredientDTO;
using RecipeApp.Application.Helpers;
using RecipeApp.Application.Interfaces;
using RecipeApp.Domain.Entities;
using RecipeApp.Infrastructure;

namespace RecipeApp.Application.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly ApplicationDbContext _db;
        public IngredientService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IngredientResponse>? AddIngredient(IngredientAddRequest ingredientAddRequest)
        {
            if (ingredientAddRequest == null)
                throw new ArgumentNullException("IngredientAddRequest nie może być null");

            bool isValid = ValidationHelper.ValidateModel(ingredientAddRequest);

            if (isValid is false)
                throw new ArgumentException("Niepoprawne wartości");

            Ingredient ingredientToAdd = ingredientAddRequest.ToIngredient();
            ingredientToAdd.ID = Guid.NewGuid();

            _db.Ingredients.Add(ingredientToAdd);
            await _db.SaveChangesAsync();

            return ingredientToAdd.ToIngredientResponse();
        }

        public async Task<bool> DeleteIngredient(Guid? ingredientID)
        {
            if (ingredientID is null)
                throw new ArgumentNullException("IngredientID nie może być null");

            Ingredient? ingredientToDelete = await _db.Ingredients.SingleOrDefaultAsync(i => i.ID == ingredientID);

            if (ingredientToDelete is null)
                return false;

            _db.Ingredients.Remove(ingredientToDelete);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<List<IngredientResponse>>? GetAllIngredients()
        {
            var ingredients = await _db.Ingredients.ToListAsync();
            return ingredients.Select(i => i.ToIngredientResponse()).ToList();
        }

        public async Task<List<IngredientResponse>>? GetFilteredIngredients(string? searchString)
        {
            var ingredients = await GetAllIngredients();
            if (searchString is null || searchString == String.Empty)
                return ingredients.ToList();

            return ingredients
                .Where(i => i.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        public async Task<IngredientResponse>? GetIngredientByID(Guid? ingredientID)
        {
            if (ingredientID is null)
                throw new ArgumentNullException("IngredientID nie może być null");

            Ingredient? ingredientFound = await _db.Ingredients.SingleOrDefaultAsync(i => i.ID == ingredientID);

            if (ingredientFound is null)
                return null;

            return ingredientFound.ToIngredientResponse();
        }

        public async Task<IngredientResponse>? UpdateIngredient(IngredientUpdateRequest ingredientUpdateRequest)
        {
            if (ingredientUpdateRequest is null || ingredientUpdateRequest.ID is null)
                throw new ArgumentNullException(nameof(ingredientUpdateRequest), "ingredientUpdateRequest nie może być null");

            bool isValid = ValidationHelper.ValidateModel(ingredientUpdateRequest);

            if (!isValid)
                throw new ArgumentException($"Obiekt ingredientUpdateRequest jest niepoprawny");

            Ingredient? ingredientFound = await _db.Ingredients.SingleOrDefaultAsync(i => i.ID == ingredientUpdateRequest.ID);

            if (ingredientFound is null)
                throw new ArgumentException($"Nie znaleziono składnika o ID:{ingredientUpdateRequest.ID}");

            ingredientFound.Name = ingredientUpdateRequest.Name;
            await _db.SaveChangesAsync();
            return ingredientFound.ToIngredientResponse();
        }
    }
}
