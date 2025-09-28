using RecipeApp.Application.DTOs;
using RecipeApp.Application.Services;
using RecipeApp.Domain.Entities;
using RecipeApp.Domain.Enums;

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
        RecipeAddRequest? recipeAddRequest = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            _recipeService.AddRecipe(recipeAddRequest);
        });
    }

    [Fact]
    public void AddRecipe_NullArguments()
    {
        RecipeAddRequest recipeAddRequest = new();

        Assert.Throws<ArgumentNullException>(() =>
        {
            _recipeService.AddRecipe(recipeAddRequest);
        });
    }

    [Fact]
    public void AddRecipe_ProperRecipeDetails()
    {
        RecipeAddRequest recipeRequestToAdd = new RecipeAddRequest
        {
            Name = "Spaghetti Bolognese",
            Description = "Klasyczne włoskie danie z makaronem spaghetti i sosem bolońskim na bazie mięsa mielonego i pomidorów.",
            Author = "Jan Kowalski",
            PrimaryCategory = PrimaryCategory.MainCourse,
            DishType = DishType.MainDish,
            PreparationTime = 45,
            Servings = 4,
            Rating = 4.8,
            ImageUrl = "https://example.com/images/spaghetti-bolognese.jpg",
            CreatedAt = DateTime.UtcNow,

            RecipeIngredients = new List<RecipeIngredient>()
            {
                new RecipeIngredient
                {
                    ID = Guid.NewGuid(),
                    RecipeID = Guid.NewGuid(), // tymczasowe, w rzeczywistości przypisane po dodaniu przepisu
                    IngredientID = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), // Makaron spaghetti
                    Quantity = 400,
                    Unit = Unit.Gram
                },
            }
        };

        RecipeResponse recipeResponseFromAdd = _recipeService.AddRecipe(recipeRequestToAdd);
        List<RecipeResponse> recipesFromGetAll = _recipeService.GetAllRecipes();

        Assert.True(recipeResponseFromAdd.ID != Guid.Empty);
        Assert.Contains(recipeResponseFromAdd, recipesFromGetAll);
    }

    [Fact]
    public void AddRecipe_MinimalProperRecipeDetails()
    {
        RecipeAddRequest recipeRequestToAdd = new RecipeAddRequest
        {
            Name = "Spaghetti Bolognese",
            Author = "Jan Kowalski",
            PrimaryCategory = PrimaryCategory.MainCourse,
            DishType = DishType.MainDish,
            CreatedAt = DateTime.UtcNow,
            RecipeIngredients = new List<RecipeIngredient>()
            {
                new RecipeIngredient
                {
                    ID = Guid.NewGuid(),
                    RecipeID = Guid.NewGuid(), // tymczasowe, w rzeczywistości przypisane po dodaniu przepisu
                    IngredientID = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), // Makaron spaghetti
                    Quantity = 400,
                    Unit = Unit.Gram
                },
            }
        };

        RecipeResponse recipeResponseFromAdd = _recipeService.AddRecipe(recipeRequestToAdd);
        List<RecipeResponse> recipesFromGetAll = _recipeService.GetAllRecipes();

        Assert.True(recipeResponseFromAdd.ID != Guid.Empty);
        Assert.Contains(recipeResponseFromAdd, recipesFromGetAll);
    }

    #endregion

    #region GetRecipeByID()

    #endregion
    #region GetAllRecipes()


    #endregion
}
