using RecipeApp.Application.DTOs;
using RecipeApp.Application.Services;

namespace RecipeApp.Tests;

public class RecipeServiceTests
{
    private readonly RecipeService _recipeService;

    public RecipeServiceTests()
    {
        _recipeService = new RecipeService();
    }

    #region AddRecipe()
    [Fact]
    public void AddRecipe_NullRecipe()
    {
        RecipeAddRequest recipeAddRequest = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            _recipeService.AddRecipe(recipeAddRequest);
        });
    }

    #endregion
}
