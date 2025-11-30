using RecipeApp.Core.Domain.Entities;
using RecipeApp.Core.Domain.RepositoriesContracts;
using RecipeApp.Core.DTOs.RecipeDTO;
using RecipeApp.Core.Helpers;
using RecipeApp.Core.ServicesContracts.RecipeContracts;

namespace RecipeApp.Core.Services.RecipeServices;

public class RecipeQueryService : IRecipeQueryService
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeQueryService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }



    public async Task<Result<RecipeResponse>> GetRecipeByID(Guid? inID)
    {
        if (inID is null)
            return Result<RecipeResponse>.Failure("Nie podano ID");

        Recipe? recipeFound = await _recipeRepository.GetRecipeByID(inID.Value);

        if (recipeFound is null)
            return Result<RecipeResponse>.Failure("Nie znaleziono szukanego przepisu.");

        return Result<RecipeResponse>.Success(recipeFound.ToRecipeResponse());
    }

    public async Task<Result<List<RecipeResponse>>> GetAllRecipes()
    {
        var recipes = await _recipeRepository.GetAllRecipes();
        List<RecipeResponse> recipesAsResponse = recipes.Select(temp => temp.ToRecipeResponse()).ToList();
        return Result<List<RecipeResponse>>.Success(recipesAsResponse);
    }
}
