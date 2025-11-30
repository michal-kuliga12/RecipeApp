using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.Helpers;
using RecipeApp.Application.Interfaces.RecipeInterfaces;

namespace RecipeApp.Application.RecipeServices;

public class RecipeService : IRecipeService
{
    private readonly IRecipeCommandService _commandService;
    private readonly IRecipeDeleteService _deleteService;
    private readonly IRecipeFilterService _filterService;
    private readonly IRecipeQueryService _queryService;

    public RecipeService(IRecipeCommandService commandService, IRecipeDeleteService deleteService, IRecipeFilterService filterService, IRecipeQueryService queryService)
    {
        _commandService = commandService;
        _deleteService = deleteService;
        _filterService = filterService;
        _queryService = queryService;
    }

    public async Task<Result<RecipeResponse>> AddRecipe(RecipeAddRequest? recipeAddRequest)
        => await _commandService.AddRecipe(recipeAddRequest);

    public async Task<Result> DeleteRecipe(Guid? recipeID)
        => await _deleteService.DeleteRecipe(recipeID);

    public async Task<Result<List<RecipeResponse>>> GetAllRecipes()
        => await _queryService.GetAllRecipes();

    public async Task<Result<List<RecipeResponse>>> GetFilteredRecipes(string seachBy, string? seachString)
        => await _filterService.GetFilteredRecipes(seachBy, seachString);

    public async Task<Result<RecipeResponse>> GetRecipeByID(Guid? recipeID)
        => await _queryService.GetRecipeByID(recipeID);

    public async Task<Result<List<RecipeResponse>>> GetSortedRecipes(List<RecipeResponse?> recipeList, string sortBy, bool ascending = true)
        => await _filterService.GetSortedRecipes(recipeList, sortBy, ascending);

    public async Task<Result<RecipeResponse>> UpdateRecipe(RecipeUpdateRequest recipeUpdateRequest)
        => await _commandService.UpdateRecipe(recipeUpdateRequest);
}
