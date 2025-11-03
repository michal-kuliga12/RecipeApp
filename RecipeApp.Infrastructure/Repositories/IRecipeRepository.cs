using RecipeApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApp.Infrastructure.Repositories
{
    public interface IRecipeRepository
    {
        Task<Recipe> AddRecipe(Recipe recipe);
        Task<Recipe?> GetRecipeByID(Guid recipeID);
        Task<List<Recipe>?> GetAllRecipes();
        Task<List<Recipe>?> GetFilteredRecipes(Expression<Func<Recipe,bool>> predicate);
        Task<Recipe> UpdateRecipe(Recipe recipe);
        Task<bool> DeleteRecipe(Guid recipeID);
    }
}
