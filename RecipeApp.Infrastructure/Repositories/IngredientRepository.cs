using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Domain.Entities;

namespace RecipeApp.Infrastructure.Repositories
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public IngredientRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Ingredient?> AddIngredient(Ingredient ingredient)
        {
            _dbContext.Ingredients.Add(ingredient);
            await _dbContext.SaveChangesAsync();
            return ingredient;
        }

        public async Task<bool> DeleteIngredient(Guid ingredientID)
        {
            Ingredient? dbIngredient = await _dbContext.Ingredients.FindAsync(ingredientID);
            if (dbIngredient is null)
                return false;

            _dbContext.Ingredients.Remove(dbIngredient);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<Ingredient>> GetAllIngredients()
        {
            return await _dbContext.Ingredients.ToListAsync();
        }

        public async Task<List<Ingredient>> GetFilteredIngredients(Expression<Func<Ingredient, bool>> predicate)
        {
            if (predicate is null)
                return await _dbContext.Ingredients.ToListAsync();

            return await _dbContext.Ingredients.Where(predicate).ToListAsync();
        }

        public async Task<Ingredient?> GetIngredientByID(Guid ingredientID)
        {
            return await _dbContext.Ingredients.FirstOrDefaultAsync(i => i.ID == ingredientID);
        }

        public async Task<Ingredient?> UpdateIngredient(Ingredient ingredient)
        {
            Ingredient? dbIngredient = await _dbContext.Ingredients.FirstOrDefaultAsync(i => i.ID == ingredient.ID);
            if (dbIngredient is null)
                return null;

            dbIngredient.Name = ingredient.Name;

            await _dbContext.SaveChangesAsync();
            return dbIngredient;
        }
    }
}
