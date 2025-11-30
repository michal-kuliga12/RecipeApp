using System.Linq.Expressions;
using RecipeApp.Domain.Entities;

namespace RecipeApp.Domain.RepositoriesContracts
{
    public interface IRecipeRepository
    {
        Task<Recipe> AddRecipe(Recipe recipe);
        Task<Recipe?> GetRecipeByID(Guid recipeID);
        Task<List<Recipe>> GetAllRecipes();
        Task<List<Recipe>> GetFilteredRecipes(Expression<Func<Recipe, bool>> predicate);
        Task<Recipe> UpdateRecipe(Recipe recipe);
        Task<bool> DeleteRecipe(Guid recipeID);
        Task<Recipe?> InsertRecipeIngredient(RecipeIngredient recipeIngredient);
    }
}
