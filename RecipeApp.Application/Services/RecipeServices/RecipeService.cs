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

    public Task<Result<RecipeResponse>> AddRecipe(RecipeAddRequest? recipeAddRequest)
        => _commandService.AddRecipe(recipeAddRequest);

    public Task<Result> DeleteRecipe(Guid? recipeID)
        => _deleteService.DeleteRecipe(recipeID);

    public Task<Result<List<RecipeResponse>>> GetAllRecipes()
        => _queryService.GetAllRecipes();

    public Task<Result<List<RecipeResponse>>> GetFilteredRecipes(string seachBy, string? seachString)
        => _filterService.GetFilteredRecipes(seachBy, seachString);

    public Task<Result<RecipeResponse>> GetRecipeByID(Guid? recipeID)
        => _queryService.GetRecipeByID(recipeID);

    public Task<Result<List<RecipeResponse>>> GetSortedRecipes(List<RecipeResponse?> recipeList, string sortBy, bool ascending = true)
        => _filterService.GetSortedRecipes(recipeList, sortBy, ascending);

    public Task<Result<RecipeResponse>> UpdateRecipe(RecipeUpdateRequest recipeUpdateRequest)
        => _commandService.UpdateRecipe(recipeUpdateRequest);
}
