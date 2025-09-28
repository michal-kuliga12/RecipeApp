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
                    IngredientID = Guid.NewGuid(), // Makaron spaghetti
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
            RecipeIngredients = new List<RecipeIngredient> { new() { ID = Guid.NewGuid(), RecipeID = Guid.NewGuid(), IngredientID = Guid.NewGuid(), Quantity = 400, Unit = Unit.Gram } }
        };


        RecipeResponse recipeResponseFromAdd = _recipeService.AddRecipe(recipeRequestToAdd);
        List<RecipeResponse> recipesFromGetAll = _recipeService.GetAllRecipes();

        Assert.True(recipeResponseFromAdd.ID != Guid.Empty);
        Assert.Contains(recipeResponseFromAdd, recipesFromGetAll);
    }

    #endregion

    #region GetRecipeByID()
    [Fact]
    public void GetRecipeByID_NullID()
    {
        Guid? invalidRecipeID = null;
        RecipeResponse? recipeFromGetByID = _recipeService.GetRecipeByID(invalidRecipeID);

        Assert.Null(recipeFromGetByID);
    }

    [Fact]
    public void GetRecipeByID_ValidID_RecipeNotFound()
    {
        Guid? invalidRecipeID = Guid.NewGuid();
        RecipeResponse? recipeFromGetByID = _recipeService.GetRecipeByID(invalidRecipeID);

        Assert.Null(recipeFromGetByID);
    }

    [Fact]
    public void GetRecipeByID_ValidID_RecipeFound()
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
                    IngredientID = Guid.NewGuid(), // Makaron spaghetti
                    Quantity = 400,
                    Unit = Unit.Gram
                },
            }
        };
        RecipeResponse recipeResponseFromAdd = _recipeService.AddRecipe(recipeRequestToAdd);
        Guid? validRecipeID = recipeResponseFromAdd.ID;

        RecipeResponse? recipeFromGetByID = _recipeService.GetRecipeByID(validRecipeID);

        Assert.NotNull(recipeFromGetByID);
        Assert.Equal(recipeFromGetByID.ID, validRecipeID);
    }

    #endregion
    #region GetAllRecipes()
    [Fact]
    public void GetAllRecipes_EmptyList()
    {
        List<RecipeResponse> recipesFromGetAll = _recipeService.GetAllRecipes();

        Assert.Empty(recipesFromGetAll);
    }
    [Fact]
    public void GetAllRecipes_AddMultipleRecipes_ShouldContainThemAll()
    {
        RecipeAddRequest recipeRequestToAdd1 = new RecipeAddRequest
        {
            Name = "Spaghetti Bolognese",
            Author = "Jan Kowalski",
            PrimaryCategory = PrimaryCategory.MainCourse,
            DishType = DishType.MainDish,
            CreatedAt = DateTime.UtcNow,
            RecipeIngredients = new List<RecipeIngredient> { new() { ID = Guid.NewGuid(), RecipeID = Guid.NewGuid(), IngredientID = Guid.NewGuid(), Quantity = 400, Unit = Unit.Gram } }
        };
        RecipeAddRequest recipeRequestToAdd2 = new RecipeAddRequest
        {
            Name = "Spaghetti Bolognese",
            Author = "Jan Kowalski",
            PrimaryCategory = PrimaryCategory.MainCourse,
            DishType = DishType.MainDish,
            CreatedAt = DateTime.UtcNow,
            RecipeIngredients = new List<RecipeIngredient> { new() { ID = Guid.NewGuid(), RecipeID = Guid.NewGuid(), IngredientID = Guid.NewGuid(), Quantity = 400, Unit = Unit.Gram } }
        };
        RecipeAddRequest recipeRequestToAdd3 = new RecipeAddRequest
        {
            Name = "Spaghetti Bolognese",
            Author = "Jan Kowalski",
            PrimaryCategory = PrimaryCategory.MainCourse,
            DishType = DishType.MainDish,
            CreatedAt = DateTime.UtcNow,
            RecipeIngredients = new List<RecipeIngredient> { new() { ID = Guid.NewGuid(), RecipeID = Guid.NewGuid(), IngredientID = Guid.NewGuid(), Quantity = 400, Unit = Unit.Gram } }
        };

        List<RecipeAddRequest> recipeRequestToAdd_List = new() { recipeRequestToAdd1, recipeRequestToAdd2, recipeRequestToAdd3 };
        List<RecipeResponse> recipeResponseFromAdd_List = new();

        var recipesToAdd = new List<RecipeAddRequest> { recipeRequestToAdd1, recipeRequestToAdd2, recipeRequestToAdd3 };
        var addedRecipes = new List<RecipeResponse>();

        // Dodanie przepisów do serwisu
        foreach (var recipeRequest in recipesToAdd)
        {
            addedRecipes.Add(_recipeService.AddRecipe(recipeRequest));
        }

        // Pobranie wszystkich przepisów
        var allRecipes = _recipeService.GetAllRecipes();

        // Sprawdzenie, czy wszystkie dodane przepisy znajdują się w kolekcji
        foreach (var addedRecipe in addedRecipes)
        {
            Assert.Contains(allRecipes, r => r.ID == addedRecipe.ID);
        }
    }
    #endregion
}