using System.ComponentModel.DataAnnotations;
using AutoFixture;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Application.DTOs.IngredientDTO;
using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.DTOs.RecipeIngredientDTO;
using RecipeApp.Application.Helpers;
using RecipeApp.Application.Interfaces;
using RecipeApp.Application.Services;
using RecipeApp.Domain.Entities;
using RecipeApp.Domain.Enums;
using RecipeApp.Infrastructure;
using RecipeApp.Tests.Helpers;

namespace RecipeApp.Tests;

public class RecipeServiceTests
{
    private readonly IRecipeService _recipeService;
    private readonly IIngredientService _ingredientService;
    private readonly IRecipeIngredientService _recipeIngredientService;
    private readonly IFixture _fixture;
    private readonly ApplicationDbContext _dbContext;

    public RecipeServiceTests()
    {
        List<Recipe>? recipesInitialData = JsonReaderHelper<Recipe>.GetJsonData("recipes");
        List<Ingredient>? ingredientsInitialData = JsonReaderHelper<Ingredient>.GetJsonData("ingredients");
        List<RecipeIngredient> recipeIngredientsInitialData = new List<RecipeIngredient>() { };


        DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options
            );

        _fixture = new Fixture();
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _dbContext = dbContextMock.Object;
        dbContextMock.CreateDbSetMock(temp => temp.Recipes, recipesInitialData);
        dbContextMock.CreateDbSetMock(temp => temp.RecipeIngredients, recipeIngredientsInitialData);
        dbContextMock.CreateDbSetMock(temp => temp.Ingredients, ingredientsInitialData);


        _recipeService = new RecipeService(_dbContext);
        _ingredientService = new IngredientService(_dbContext);
        _recipeIngredientService = new RecipeIngredientService(_recipeService, _ingredientService, _dbContext);
    }

    #region AddRecipe()
    [Fact]
    public async Task AddRecipe_NullRecipe()
    {
        RecipeAddRequest? recipeAddRequest = null;

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _recipeService.AddRecipe(recipeAddRequest);
        });
    }

    [Fact]
    public async Task AddRecipe_NullArguments()
    {
        RecipeAddRequest recipeAddRequest = new();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _recipeService.AddRecipe(recipeAddRequest);
        });
    }

    [Fact]
    public async Task AddRecipe_ProperRecipeDetails()
    {
        RecipeAddRequest recipeRequestToAdd = _fixture.Create<RecipeAddRequest>();

        RecipeResponse? recipeResponseFromAdd = await _recipeService.AddRecipe(recipeRequestToAdd);
        List<RecipeResponse>? recipesFromGetAll = await _recipeService.GetAllRecipes();

        Assert.True(recipeResponseFromAdd.ID != Guid.Empty);
        Assert.Contains(recipeResponseFromAdd, recipesFromGetAll);
    }

    [Fact]
    public async Task AddRecipe_MinimalProperRecipeDetails()
    {
        RecipeAddRequest recipeRequestToAdd = _fixture.Build<RecipeAddRequest>()
            .OmitAutoProperties()
            .With(x => x.Name, "Spaghetti Bolognese")
            .With(x => x.Author, "Jan Kowalski")
            .With(x => x.Category, Category.MainCourse)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .With(x => x.RecipeIngredients, new List<RecipeIngredient> { new() { ID = Guid.NewGuid(), RecipeID = Guid.NewGuid(), IngredientID = Guid.NewGuid(), Quantity = 400, Unit = Unit.Gram } })
            .Create();

        RecipeResponse recipeResponseFromAdd = await _recipeService.AddRecipe(recipeRequestToAdd);
        List<RecipeResponse> recipesFromGetAll = await _recipeService.GetAllRecipes();

        Assert.True(recipeResponseFromAdd.ID != Guid.Empty);
        Assert.Contains(recipeResponseFromAdd, recipesFromGetAll);
    }
    #endregion
    #region GetRecipeByID()
    [Fact]
    public async Task GetRecipeByID_NullID()
    {
        Guid? invalidRecipeID = null;
        RecipeResponse? recipeFromGetByID = await _recipeService.GetRecipeByID(invalidRecipeID);

        Assert.Null(recipeFromGetByID);
    }

    [Fact]
    public async Task GetRecipeByID_ValidID_RecipeNotFound()
    {
        Guid? invalidRecipeID = Guid.NewGuid();
        RecipeResponse? recipeFromGetByID = await _recipeService.GetRecipeByID(invalidRecipeID);

        Assert.Null(recipeFromGetByID);
    }

    [Fact]
    public async Task GetRecipeByID_ValidID_RecipeFound()
    {
        List<RecipeResponse>? allRecipes = await _recipeService.GetAllRecipes();
        RecipeResponse recipeResponseFromAdd = allRecipes.First();
        Guid? validRecipeID = recipeResponseFromAdd.ID;

        RecipeResponse? recipeFromGetByID = await _recipeService.GetRecipeByID(validRecipeID);

        Assert.NotNull(recipeFromGetByID);
        Assert.Equal(recipeFromGetByID.ID, validRecipeID);
    }

    #endregion
    #region GetAllRecipes()

    [Fact]
    public async Task GetAllRecipes_AddMultipleRecipes_ShouldContainThemAll()
    {
        RecipeAddRequest recipeRequestToAdd1 = _fixture.Create<RecipeAddRequest>();
        RecipeAddRequest recipeRequestToAdd2 = _fixture.Create<RecipeAddRequest>();
        RecipeAddRequest recipeRequestToAdd3 = _fixture.Create<RecipeAddRequest>();

        List<RecipeAddRequest> recipeRequestToAdd_List = new() { recipeRequestToAdd1, recipeRequestToAdd2, recipeRequestToAdd3 };
        List<RecipeResponse> recipeResponseFromAdd_List = new();

        var recipesToAdd = new List<RecipeAddRequest> { recipeRequestToAdd1, recipeRequestToAdd2, recipeRequestToAdd3 };
        var addedRecipes = new List<RecipeResponse>();

        // Dodanie przepisów do serwisu
        foreach (var recipeRequest in recipesToAdd)
        {
            addedRecipes.Add(await _recipeService.AddRecipe(recipeRequest));
        }

        // Pobranie wszystkich przepisów
        var allRecipes = await _recipeService.GetAllRecipes();

        // Sprawdzenie, czy wszystkie dodane przepisy znajdują się w kolekcji
        foreach (var addedRecipe in addedRecipes)
        {
            Assert.Contains(allRecipes, r => r.ID == addedRecipe.ID);
        }
    }
    #endregion
    #region GetFilteredRecipes()
    [Fact]
    public async Task GetFilteredRecipes_ProperSearchValues()
    {
        List<RecipeResponse>? allRecipes = await _recipeService.GetAllRecipes();
        List<RecipeResponse> expectedFilteredRecipes = new List<RecipeResponse>();

        string? searchString = "Spaghetti";
        string? searchBy = nameof(RecipeResponse.Name);
        List<RecipeResponse> filteredRecipes = new List<RecipeResponse>();

        filteredRecipes = await _recipeService.GetFilteredRecipes(searchBy, searchString);
        expectedFilteredRecipes = allRecipes.Where(
            r => r.Name != null
            && r.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
        ).ToList();

        Assert.Equal(expectedFilteredRecipes.Count, filteredRecipes.Count);
        Assert.NotEmpty(filteredRecipes);
        Assert.All(filteredRecipes, r => Assert.Contains(searchString, r.Name, StringComparison.OrdinalIgnoreCase));
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetFilteredRecipes_NullOrEmptySearchName(string searchString)
    {
        List<RecipeResponse> allRecipes = await _recipeService.GetAllRecipes();

        string? searchBy = nameof(RecipeResponse.Name);
        List<RecipeResponse> filteredRecipes = await _recipeService.GetFilteredRecipes(searchBy, searchString);

        Assert.Equal(allRecipes, filteredRecipes);
    }

    [Fact]
    public async Task GetFilteredRecipes_NullSearchBy()
    {
        List<RecipeResponse> allRecipes = await _recipeService.GetAllRecipes();
        string? searchString = "sałatka";
        string? searchBy = null;

        List<RecipeResponse> filteredRecipes = await _recipeService.GetFilteredRecipes(searchBy, searchString);

        Assert.Equal(allRecipes.Count, filteredRecipes.Count);
    }

    [Fact]
    public async Task GetFilteredRecipes_NoResults()
    {
        List<RecipeResponse> allRecipes = await _recipeService.GetAllRecipes();

        string? searchString = "Paella z kurczakiem";
        string? searchBy = nameof(RecipeResponse.Name);
        List<RecipeResponse> filteredRecipes = await _recipeService.GetFilteredRecipes(searchBy, searchString);

        Assert.Empty(filteredRecipes);
    }

    #endregion
    #region GetSortedRecipes()
    [Fact]
    public async Task GetSortedRecipes_ReturnAscendingOrder()
    {
        List<RecipeResponse> allRecipes = await _recipeService.GetAllRecipes();

        List<RecipeResponse>? sortedRecipes = await _recipeService.GetSortedRecipes(allRecipes, nameof(RecipeResponse.Author), true);

        var expectedOrder = allRecipes.OrderBy(r => r.Author).ToList();
        Assert.Equal(expectedOrder.Select(r => r.Author), sortedRecipes.Select(r => r.Author));
    }

    [Fact]
    public async Task GetSortedRecipes_ReturnDescendingOrder()
    {
        List<RecipeResponse> allRecipes = await _recipeService.GetAllRecipes();

        List<RecipeResponse>? sortedRecipes = await _recipeService.GetSortedRecipes(allRecipes, nameof(RecipeResponse.Author), false);

        var expectedOrder = allRecipes.OrderByDescending(r => r.Author).ToList();
        Assert.Equal(expectedOrder.Select(r => r.Author), sortedRecipes.Select(r => r.Author));
    }

    [Fact]
    public async Task GetSortedRecipes_CheckCount()
    {
        List<RecipeResponse> allRecipes = await _recipeService.GetAllRecipes();

        List<RecipeResponse>? sortedRecipes = await _recipeService.GetSortedRecipes(allRecipes, nameof(RecipeResponse.Author), false);

        Assert.Equal(sortedRecipes.Count, allRecipes.Count);
    }

    [Fact]
    public async Task GetSortedRecipes_EmptyList_ReturnEmpty()
    {
        List<RecipeResponse>? emptyRecipesList = await _recipeService.GetSortedRecipes(new List<RecipeResponse>(), nameof(RecipeResponse.Author), false);
        Assert.Empty(emptyRecipesList);
    }

    [Fact]
    public async Task GetSortedRecipes_SingleElement_ReturnsSameList()
    {
        List<RecipeResponse> single = new List<RecipeResponse> { new RecipeResponse { Author = "Test" } };

        List<RecipeResponse>? result = await _recipeService.GetSortedRecipes(single, nameof(RecipeResponse.Author), true);

        Assert.Single(result);
        Assert.Equal("Test", result[0].Author);
    }

    [Fact]
    public async Task GetSortedRecipes_InvalidProperty_ReturnUnsorted()
    {
        List<RecipeResponse> allRecipes = await _recipeService.GetAllRecipes();

        List<RecipeResponse> sortedRecipes = await _recipeService.GetSortedRecipes(allRecipes, "abc", false);

        Assert.Equal(allRecipes.Count, sortedRecipes.Count);
    }

    [Fact]
    public async Task GetSortedRecipes_NullList_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _recipeService.GetSortedRecipes(null, nameof(RecipeResponse.Author), true));
    }
    #endregion
    #region UpdateRecipe()
    [Fact]
    public async Task UpdateRecipe_NullRecipe()
    {
        RecipeUpdateRequest? recipeUpdateRequest = null;
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _recipeService.UpdateRecipe(recipeUpdateRequest);
        });
    }

    [Fact]
    public async Task UpdateRecipe_NullID()
    {
        RecipeUpdateRequest recipeUpdateRequest = _fixture.Build<RecipeUpdateRequest>()
            .With(x => x.ID, Guid.Empty)
            .Create();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _recipeService.UpdateRecipe(recipeUpdateRequest);
        });
    }

    [Fact]
    public async Task UpdateRecipe_ValidData_UpdatesSuccessfully()
    {
        List<RecipeResponse> allRecipes = await _recipeService.GetAllRecipes();
        RecipeResponse existingRecipe = allRecipes.First();

        RecipeUpdateRequest recipeToUpdate = _fixture.Build<RecipeUpdateRequest>()
            .With(x => x.ID, existingRecipe.ID)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Create();

        RecipeResponse updatedRecipe = await _recipeService.UpdateRecipe(recipeToUpdate);
        RecipeResponse? recipeFromGetByID = await _recipeService.GetRecipeByID(existingRecipe.ID);

        // Assert
        Assert.NotNull(updatedRecipe);
        Assert.Equal(existingRecipe.ID, updatedRecipe.ID);
        Assert.Equal(recipeToUpdate.Name, updatedRecipe.Name);
        Assert.Equal(recipeToUpdate.Description, updatedRecipe.Description);
        Assert.Equal(recipeToUpdate.Author, updatedRecipe.Author);
        Assert.Equal(recipeToUpdate.Category, updatedRecipe.Category);
        Assert.Equal(recipeToUpdate.PreparationTime, updatedRecipe.PreparationTime);
        Assert.Equal(recipeToUpdate.Servings, updatedRecipe.Servings);
        Assert.Equal(recipeToUpdate.Rating, updatedRecipe.Rating);
        Assert.Equal(recipeToUpdate.ImageUrl, updatedRecipe.ImageUrl);
        Assert.Equal(existingRecipe.CreatedAt, updatedRecipe.CreatedAt);

        Assert.NotNull(recipeFromGetByID);
        Assert.Equal(recipeToUpdate.Name, recipeFromGetByID.Name);
        Assert.Equal(recipeToUpdate.Rating, recipeFromGetByID.Rating);
    }

    #endregion
    #region DeleteRecipe()
    [Fact]
    public async Task DeleteRecipe_NullID()
    {
        Guid? invalidRecipeID = null;
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _recipeService.DeleteRecipe(invalidRecipeID);
        });
    }

    [Fact]
    public async Task DeleteRecipe_InvalidID_ReturnsFalse()
    {
        Guid? invalidRecipeID = Guid.NewGuid();
        bool result = await _recipeService.DeleteRecipe(invalidRecipeID);
        Assert.False(result);
    }

    public async Task DeleteRecipe_ValidID_DeletesSuccessfully()
    {

        List<RecipeResponse> allRecipes = await _recipeService.GetAllRecipes();
        RecipeResponse addedRecipe = allRecipes.First();
        Guid? validRecipeID = addedRecipe.ID;

        bool deleteResult = await _recipeService.DeleteRecipe(validRecipeID);
        RecipeResponse? recipeFromGetByID = await _recipeService.GetRecipeByID(validRecipeID);

        Assert.True(deleteResult);
        Assert.Null(recipeFromGetByID);
        Assert.DoesNotContain(addedRecipe, allRecipes);
    }
    #endregion

    #region AddRecipeIngredient()
    [Fact]
    public async Task AddRecipeIngredient_ValidRequest()
    {
        RecipeResponse? recipe = await _recipeService.AddRecipe(_fixture.Create<RecipeAddRequest>());
        Guid recipeID = recipe.ID;

        IngredientResponse? ingredient = await _ingredientService.AddIngredient(_fixture.Create<IngredientAddRequest>());
        Guid ingredientID = ingredient.ID;

        RecipeIngredientAddRequest recipeAddIngredientRequest = new()
        {
            IngredientID = ingredientID,
            RecipeID = recipeID,
            Quantity = 1,
            Unit = Unit.Piece
        };

        RecipeResponse? recipeWithAddedIngredient = await _recipeIngredientService.AddRecipeIngredient(recipeAddIngredientRequest);

        Assert.Single(recipeWithAddedIngredient.RecipeIngredients);
        var addedIngredient = recipeWithAddedIngredient.RecipeIngredients.First();

        Assert.Equal(recipeAddIngredientRequest.IngredientID, addedIngredient.IngredientID);
        Assert.Equal(recipeAddIngredientRequest.RecipeID, addedIngredient.RecipeID);
        Assert.Equal(recipeAddIngredientRequest.Quantity, addedIngredient.Quantity);
        Assert.Equal(recipeAddIngredientRequest.Unit, addedIngredient.Unit);

    }

    [Fact]
    public async Task AddRecipeIngredient_InvalidQuantity_ShouldFailValidation()
    {
        RecipeIngredientAddRequest request = _fixture.Build<RecipeIngredientAddRequest>()
            .With(x => x.Quantity, 0)
            .Create();

        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(request);
        bool isValid = Validator.TryValidateObject(request, context, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, r => r.ErrorMessage.Contains("Ilość musi być większa niż 0 i mniejsza niż 10 000"));
    }

    [Fact]
    public async Task AddRecipeIngredient_InvalidIngredientID()
    {
        // Arrange
        RecipeIngredientAddRequest request = _fixture.Build<RecipeIngredientAddRequest>()
            .With(x => x.IngredientID, Guid.Empty)
            .Create();

        // Act
        var response = _recipeIngredientService.AddRecipeIngredient(request);

        Assert.Null(response);
    }
    [Fact]
    public async Task AddRecipeIngredient_EmptyRecipeID_ReturnsError()
    {
        // Arrange
        RecipeIngredientAddRequest request = _fixture.Build<RecipeIngredientAddRequest>()
            .With(x => x.RecipeID, Guid.Empty)
            .Create();

        // Act
        var response = _recipeIngredientService.AddRecipeIngredient(request);

        Assert.Null(response);
    }

    [Fact]
    public async Task AddRecipeIngredient_MissingUnit_ShouldFailValidation()
    {
        RecipeIngredientAddRequest request = _fixture.Build<RecipeIngredientAddRequest>()
            .With(x => x.Unit, (Unit?)null)
            .Create();

        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(request);
        bool isValid = Validator.TryValidateObject(request, context, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, r => r.ErrorMessage.Contains("Jednostka jest wymagana"));
    }

    [Fact]
    public async Task AddRecipeIngredient_AlreadyExists_ReturnsError()
    {
        Recipe recipe = _fixture.Create<Recipe>();
        _dbContext.Recipes.Add(recipe);

        Ingredient ingredient = _fixture.Create<Ingredient>();
        _dbContext.Ingredients.Add(ingredient);
        await _dbContext.SaveChangesAsync();

        RecipeIngredientAddRequest recipeAddIngredientRequest = new()
        {
            IngredientID = ingredient.ID,
            RecipeID = recipe.ID,
            Quantity = 1,
            Unit = Unit.Piece
        };

        RecipeResponse? recipeWithAddedIngredient = await _recipeIngredientService.AddRecipeIngredient(recipeAddIngredientRequest);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _recipeIngredientService.AddRecipeIngredient(recipeAddIngredientRequest);
        });
    }

    #endregion
    #region UpdateRecipeIngredient()
    [Fact]
    public async Task UpdateRecipeIngredient_ValidRequest()
    {
        // Arrange
        Ingredient ingredient = _fixture.Create<Ingredient>();
        _dbContext.Ingredients.Add(ingredient);
        Recipe recipe = _fixture.Build<Recipe>()
            .With(x => x.RecipeIngredients, new List<RecipeIngredient>())
            .Create();
        _dbContext.Recipes.Add(recipe);
        await _dbContext.SaveChangesAsync();

        RecipeIngredientAddRequest recipeAddIngredientRequest = _fixture.Build<RecipeIngredientAddRequest>()
            .With(x => x.IngredientID, ingredient.ID)
            .With(x => x.RecipeID, recipe.ID)
            .Create();

        RecipeResponse? recipeWithIngredient = await _recipeIngredientService.AddRecipeIngredient(recipeAddIngredientRequest);

        //Act
        RecipeIngredient? addedIngredient = recipeWithIngredient.RecipeIngredients.First();

        RecipeIngredientUpdateRequest recipeUpdateIngredientRequest = _fixture.Build<RecipeIngredientUpdateRequest>()
            .With(x => x.ID, addedIngredient.ID)
            .With(x => x.IngredientID, ingredient.ID)
            .With(x => x.RecipeID, recipe.ID)
            .Create();

        RecipeResponse? recipeWithUpdatedIngredient = await _recipeIngredientService.UpdateRecipeIngredient(recipeUpdateIngredientRequest);

        RecipeIngredient? updatedIngredient = recipeWithUpdatedIngredient.RecipeIngredients.First();

        Assert.NotNull(addedIngredient);
        Assert.NotEmpty(recipeWithIngredient.RecipeIngredients);
        Assert.NotNull(updatedIngredient);
        Assert.NotEmpty(recipeWithUpdatedIngredient.RecipeIngredients);
        Assert.Equal(addedIngredient.ID, updatedIngredient.ID);
        Assert.Equal(addedIngredient.IngredientID, updatedIngredient.IngredientID);
        Assert.Equal(addedIngredient.RecipeID, updatedIngredient.RecipeID);
        Assert.Equal(recipeUpdateIngredientRequest.Unit, updatedIngredient.Unit);
        Assert.Equal(recipeUpdateIngredientRequest.Quantity, updatedIngredient.Quantity);
    }

    [Fact]
    public async Task UpdateRecipeIngredient_InvalidOperation_WhenRecipeIngredientNotFound()
    {
        Recipe addedRecipe = _fixture.Create<Recipe>();
        _dbContext.Recipes.Add(addedRecipe);

        Ingredient addedIngredient = _fixture.Create<Ingredient>();
        _dbContext.Ingredients.Add(addedIngredient);
        await _dbContext.SaveChangesAsync();

        RecipeIngredientUpdateRequest request = new RecipeIngredientUpdateRequest
        {
            ID = Guid.NewGuid(),
            IngredientID = addedIngredient.ID,
            RecipeID = addedRecipe.ID,
            Quantity = 5,
            Unit = Unit.Gram
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _recipeIngredientService.UpdateRecipeIngredient(request);
        });
    }

    [Fact]
    public async Task UpdateRecipeIngredient_InvalidOperation_WhenIngredientNotFound()
    {
        Recipe addedRecipe = _fixture.Create<Recipe>();
        _dbContext.Recipes.Add(addedRecipe);

        Ingredient addedIngredient = _fixture.Create<Ingredient>();
        _dbContext.Ingredients.Add(addedIngredient);
        await _dbContext.SaveChangesAsync();

        RecipeIngredientUpdateRequest request = _fixture.Build<RecipeIngredientUpdateRequest>()
            .With(x => x.ID, Guid.NewGuid())
            .With(x => x.IngredientID, addedIngredient.ID)
            .With(x => x.RecipeID, addedRecipe.ID)
            .Create();

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _recipeIngredientService.UpdateRecipeIngredient(request);
        });
    }
    [Fact]
    public async Task UpdateRecipeIngredient_InvalidOperation_WhenRecipeNotFound()
    {
        Ingredient addedIngredient = _fixture.Create<Ingredient>();
        _dbContext.Ingredients.Add(addedIngredient);
        await _dbContext.SaveChangesAsync();

        RecipeIngredientUpdateRequest request = _fixture.Build<RecipeIngredientUpdateRequest>()
            .With(x => x.ID, Guid.NewGuid())
            .With(x => x.IngredientID, addedIngredient.ID)
            .With(x => x.RecipeID, Guid.NewGuid())
            .Create();

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _recipeIngredientService.UpdateRecipeIngredient(request);
        });
    }

    public static IEnumerable<object[]> InvalidGuids =>
    [
        new object[] { null, Guid.NewGuid(), Guid.NewGuid() },
        new object[] { Guid.Empty, Guid.NewGuid(), Guid.NewGuid() },

        new object[] { Guid.NewGuid(), null, Guid.NewGuid() },
        new object[] { Guid.NewGuid(), Guid.Empty, Guid.NewGuid() },

        new object[] { Guid.NewGuid(), Guid.NewGuid(), null },
        new object[] { Guid.NewGuid(), Guid.NewGuid(), Guid.Empty }
    ];


    [Theory]
    [MemberData(nameof(InvalidGuids))]
    public async Task UpdateRecipeIngredient_ShouldFail_IDValidationFailed(Guid? id, Guid? ingredientId, Guid? recipeId)
    {
        // Arrange
        var request = new RecipeIngredientUpdateRequest
        {
            ID = id,
            IngredientID = ingredientId,
            RecipeID = recipeId,
            Quantity = 10,
            Unit = Unit.Gram
        };

        // Act
        bool isValid = ValidationHelper.ValidateModel(request);

        // Assert
        Assert.False(isValid);
    }

    public async Task UpdateRecipeIngredient_NullRequest_ThrowException(Guid? id, Guid? ingredientId, Guid? recipeId)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _recipeIngredientService.UpdateRecipeIngredient(null);
        });
    }
    // Update recipe
    // request ?= null
    // recipe get by id
    // recipe ?= null
    // walidacja
    // get recipeingredient by ID
    // aktualizacja recipe ingredient
    // return recipe response
    #endregion
    #region DeleteRecipeIngredient()
    [Fact]
    public async Task DeleteRecipeIngredient_NullID_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _recipeIngredientService.DeleteRecipeIngredient(null);
        });
    }

    [Fact]
    public async Task DeleteRecipeIngredient_InvalidID_ReturnsFalse()
    {
        Guid? invalidRecipeIngredientID = Guid.NewGuid();
        bool result = await _recipeService.DeleteRecipe(invalidRecipeIngredientID);
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteRecipeIngredient_ValidID_DeletesSuccessfully()
    {
        // Arrange
        Recipe addedRecipe = _fixture.Build<Recipe>()
            .With(x => x.RecipeIngredients, new List<RecipeIngredient>())
            .Create();

        Ingredient addedIngredient = _fixture.Create<Ingredient>();

        RecipeIngredient addedRecipeIngredient = _fixture.Build<RecipeIngredient>()
            .With(x => x.ID, Guid.NewGuid())
            .With(x => x.IngredientID, addedIngredient.ID)
            .With(x => x.RecipeID, addedRecipe.ID)
            .Create();

        _dbContext.RecipeIngredients.Add(addedRecipeIngredient);
        addedRecipe.RecipeIngredients.Add(addedRecipeIngredient);
        _dbContext.Recipes.Add(addedRecipe);
        _dbContext.Ingredients.Add(addedIngredient);
        await _dbContext.SaveChangesAsync();

        // Act
        bool deleteResult = await _recipeIngredientService.DeleteRecipeIngredient(addedRecipeIngredient.ID);

        // Assert
        Assert.True(deleteResult);
        Recipe? recipeFromDb = await _dbContext.Recipes.SingleOrDefaultAsync(x => x.ID == addedRecipe.ID);
        List<RecipeIngredient> recipeIngredients = recipeFromDb.RecipeIngredients;
        Assert.Empty(recipeIngredients);
    }
}
#endregion