using RecipeApp.Application.Helpers;
using RecipeApp.Application.Interfaces.RecipeInterfaces;
using RecipeApp.Domain.Entities;
using RecipeApp.Domain.RepositoriesContracts;

namespace RecipeApp.Application.RecipeServices;

public class RecipeDeleteService : IRecipeDeleteService
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeDeleteService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public async Task<Result> DeleteRecipe(Guid? inID)
    {
        if (inID is null)
            throw new ArgumentNullException("RecipeID nie może być null");

        Recipe? recipeToDelete = await _recipeRepository.GetRecipeByID(inID.Value);

        if (recipeToDelete is null)
            return Result.Failure("Nie znaleziono przepisu w bazie danych");

        await _recipeRepository.DeleteRecipe(inID.Value);

        return Result.Success();
    }
}
