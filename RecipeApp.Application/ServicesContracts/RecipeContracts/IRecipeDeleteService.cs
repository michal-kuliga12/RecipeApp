using RecipeApp.Core.Helpers;

namespace RecipeApp.Core.ServicesContracts.RecipeContracts;

public interface IRecipeDeleteService
{
    Task<Result> DeleteRecipe(Guid? recipeID);
}
