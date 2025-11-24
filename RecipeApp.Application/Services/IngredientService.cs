using System.Linq.Expressions;
using RecipeApp.Application.DTOs.IngredientDTO;
using RecipeApp.Application.Helpers;
using RecipeApp.Application.Interfaces;
using RecipeApp.Domain.Entities;
using RecipeApp.Infrastructure.Repositories;

namespace RecipeApp.Application.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _ingredientRepository;
        public IngredientService(IIngredientRepository ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        public async Task<Result<IngredientResponse>> AddIngredient(IngredientAddRequest ingredientAddRequest)
        {
            if (ingredientAddRequest is null)
                throw new ArgumentNullException("IngredientAddRequest nie może być null");

            bool isValid = ValidationHelper.ValidateModel(ingredientAddRequest);

            if (!isValid)
                return Result<IngredientResponse>.Failure("Wprowadzono niepoprawne dane");

            Ingredient newIngredient = ingredientAddRequest.ToIngredient();
            newIngredient.ID = Guid.NewGuid();

            await _ingredientRepository.AddIngredient(newIngredient);

            return Result<IngredientResponse>.Success(newIngredient.ToIngredientResponse());
        }

        public async Task<Result> DeleteIngredient(Guid? inID)
        {
            if (inID is null)
                return Result.Failure("Nie podano ID.");

            Ingredient? ingredientToDelete = await _ingredientRepository.GetIngredientByID(inID.Value);

            if (ingredientToDelete is null)
                return Result.Failure("Nie znaleziono szukanego składnika."); ;

            await _ingredientRepository.DeleteIngredient(ingredientToDelete.ID);

            return Result.Success();
        }

        public async Task<Result<List<IngredientResponse>>> GetAllIngredients()
        {
            var ingredients = await _ingredientRepository.GetAllIngredients();
            return Result<List<IngredientResponse>>.Success(ingredients.Select(i => i.ToIngredientResponse()).ToList());
        }

        public async Task<Result<List<IngredientResponse>>> GetFilteredIngredients(string? searchString)
        {
            var ingredients = new List<Ingredient>();

            if (string.IsNullOrWhiteSpace(searchString))
            {
                var all = await _ingredientRepository.GetAllIngredients();
                return Result<List<IngredientResponse>>.Success(all.Select(i => i.ToIngredientResponse()).ToList());
            }

            Expression<Func<Ingredient, bool>> filter = i => i.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase);
            var filtered = await _ingredientRepository.GetFilteredIngredients(filter) ?? new List<Ingredient>();

            return Result<List<IngredientResponse>>.Success(filtered.Select(i => i.ToIngredientResponse()).ToList());
        }

        public async Task<Result<IngredientResponse>> GetIngredientByID(Guid? inID)
        {
            if (inID is null)
                return Result<IngredientResponse>.Failure("Nie podano ID.");

            var dbIngredient = await _ingredientRepository.GetIngredientByID(inID.Value);

            if (dbIngredient is null)
                return Result<IngredientResponse>.Failure("Nie znaleziono szukanego składnika.");

            return Result<IngredientResponse>.Success(dbIngredient.ToIngredientResponse());
        }

        public async Task<Result<IngredientResponse>> UpdateIngredient(IngredientUpdateRequest ingredientUpdateRequest)
        {
            if (ingredientUpdateRequest is null)
                throw new ArgumentNullException(nameof(ingredientUpdateRequest), "ingredientUpdateRequest nie może być null");

            bool isModelValid = ValidationHelper.ValidateModel(ingredientUpdateRequest);

            if (!isModelValid)
                return Result<IngredientResponse>.Failure("Wprowadzono niepoprawne dane");

            var dbIngredient = await _ingredientRepository.GetIngredientByID(ingredientUpdateRequest.ID.Value);

            if (dbIngredient is null)
                return Result<IngredientResponse>.Failure("Nie znaleziono szukanego składnika.");

            dbIngredient.Name = ingredientUpdateRequest.Name;

            await _ingredientRepository.UpdateIngredient(dbIngredient);
            return Result<IngredientResponse>.Success(dbIngredient.ToIngredientResponse());
        }
    }
}
