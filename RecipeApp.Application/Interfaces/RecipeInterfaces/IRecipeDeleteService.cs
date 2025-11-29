using RecipeApp.Application.Helpers;

namespace RecipeApp.Application.Interfaces.RecipeInterfaces;

public interface IRecipeDeleteService
{
    Task<Result> DeleteRecipe(Guid? recipeID);
}
