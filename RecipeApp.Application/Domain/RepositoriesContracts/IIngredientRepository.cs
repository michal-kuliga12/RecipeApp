using System.Linq.Expressions;
using RecipeApp.Core.Domain.Entities;

namespace RecipeApp.Core.Domain.RepositoriesContracts

{
    public interface IIngredientRepository
    {
        Task<Ingredient?> AddIngredient(Ingredient ingredient);
        Task<Ingredient?> GetIngredientByID(Guid ingredientID);
        Task<List<Ingredient>> GetAllIngredients();
        Task<List<Ingredient>> GetFilteredIngredients(Expression<Func<Ingredient, bool>> predicate);
        Task<Ingredient?> UpdateIngredient(Ingredient ingredient);
        Task<bool> DeleteIngredient(Guid ingredientID);
    }
}
