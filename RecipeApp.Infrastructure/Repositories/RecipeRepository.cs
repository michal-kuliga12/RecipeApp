using Microsoft.EntityFrameworkCore;
using RecipeApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApp.Infrastructure.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public RecipeRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Recipe> AddRecipe(Recipe recipe)
        {
            _dbContext.Recipes.Add(recipe);
            await _dbContext.SaveChangesAsync();
            return recipe;
        }

        public async Task<bool> DeleteRecipe(Guid recipeID)
        {
            Recipe? recipeToDelete = await _dbContext.Recipes.FindAsync(recipeID);
            if (recipeToDelete == null)
                return false;

            _dbContext.Recipes.Remove(recipeToDelete);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<Recipe>?> GetAllRecipes()
        {
            return await _dbContext.Recipes.ToListAsync();
        }

        public async Task<List<Recipe>?> GetFilteredRecipes(Expression<Func<Recipe, bool>> predicate)
        {
            return await _dbContext.Recipes
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Recipe?> GetRecipeByID(Guid recipeID)
        {
            Recipe? recipe = await _dbContext.Recipes.SingleOrDefaultAsync(r => r.ID == recipeID);
            return recipe;
        }

        public async Task<Recipe> UpdateRecipe(Recipe recipe)
        {
            Recipe? existingRecipe = await _dbContext.Recipes.FirstOrDefaultAsync(temp => temp.ID == recipe.ID);
            if (existingRecipe == null)
                return recipe;

            recipe.Name = existingRecipe.Name;
            recipe.Description = existingRecipe.Description;
            recipe.Author = existingRecipe.Author;
            recipe.Category = existingRecipe.Category;
            recipe.PreparationTime = existingRecipe.PreparationTime;
            recipe.RecipeIngredients = existingRecipe.RecipeIngredients;
            recipe.Servings = existingRecipe.Servings;
            recipe.Rating = existingRecipe.Rating;
            recipe.ImageUrl = existingRecipe.ImageUrl;


            int countUpdated = await _dbContext.SaveChangesAsync();
            return recipe;
        }
    }
}
