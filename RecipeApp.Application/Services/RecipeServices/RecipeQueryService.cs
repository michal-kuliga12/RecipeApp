using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.Helpers;
using RecipeApp.Application.Interfaces.RecipeInterfaces;
using RecipeApp.Domain.Entities;
using RecipeApp.Infrastructure.Repositories;

namespace RecipeApp.Application.RecipeServices;

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
