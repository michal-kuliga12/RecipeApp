using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
using RecipeApp.Application.DTOs.IngredientDTO;
using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.DTOs.RecipeIngredientDTO;
using RecipeApp.Application.Helpers;
using RecipeApp.Application.Interfaces;
using RecipeApp.Application.Services;
using RecipeApp.Domain.Entities;
using RecipeApp.Domain.Enums;
using RecipeApp.Infrastructure;
using RecipeApp.Infrastructure.Repositories;

namespace RecipeApp.Tests;

public class RecipeServiceTests
{
    private readonly IRecipeService _recipeService;
    private readonly IIngredientService _ingredientService;
    private readonly IRecipeIngredientService _recipeIngredientService;
    private readonly IFixture _fixture;
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
    private readonly IRecipeRepository _recipeRepository;

    private readonly ApplicationDbContext _dbContext;

    public RecipeServiceTests()
    {
        _recipeRepositoryMock = new Mock<IRecipeRepository>();
        _recipeRepository = _recipeRepositoryMock.Object;

        _fixture = new Fixture();
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _recipeService = new RecipeService(_recipeRepository);
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

        _recipeRepositoryMock.Setup(r => r.AddRecipe(It.IsAny<Recipe>()))
            .ReturnsAsync((Recipe req) =>
            {
                return new Recipe
                {
                    ID = Guid.NewGuid(),
                    Name = req.Name,
                    Description = req.Description,
                    Author = req.Author,
                    Category = req.Category,
                    PreparationTime = req.PreparationTime,
                    Servings = req.Servings,
                    Rating = req.Rating,
                    ImageUrl = req.ImageUrl,
                    CreatedAt = req.CreatedAt,
                    RecipeIngredients = req.RecipeIngredients
                };
            });

        RecipeResponse? recipeResponseFromAdd = await _recipeService.AddRecipe(recipeRequestToAdd);

        Assert.True(recipeResponseFromAdd.ID != Guid.Empty);
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

        _recipeRepositoryMock.Setup(r => r.AddRecipe(It.IsAny<Recipe>()))
    .ReturnsAsync((Recipe req) =>
    {
        return new Recipe
        {
            ID = Guid.NewGuid(),
            Name = req.Name,
            Description = req.Description,
            Author = req.Author,
            Category = req.Category,
            PreparationTime = req.PreparationTime,
            Servings = req.Servings,
            Rating = req.Rating,
            ImageUrl = req.ImageUrl,
            CreatedAt = req.CreatedAt,
            RecipeIngredients = req.RecipeIngredients
        };
    });

        RecipeResponse recipeResponseFromAdd = await _recipeService.AddRecipe(recipeRequestToAdd);

        Assert.True(recipeResponseFromAdd.ID != Guid.Empty);
    }
    #endregion
    #region GetRecipeByID()
    [Fact]
    public async Task GetRecipeByID_NullID()
    {
        Guid? invalidRecipeID = null;

        _recipeRepositoryMock.Setup(r => r.GetRecipeByID(It.IsAny<Guid>()))
            .ReturnsAsync((Recipe?)null);

        RecipeResponse? recipeFromGetByID = await _recipeService.GetRecipeByID(invalidRecipeID);

        Assert.Null(recipeFromGetByID);
    }

    [Fact]
    public async Task GetRecipeByID_ValidID_RecipeNotFound()
    {
        Guid? invalidRecipeID = Guid.NewGuid();

        _recipeRepositoryMock.Setup(r => r.GetRecipeByID(It.IsAny<Guid>()))
            .ReturnsAsync((Recipe?)null);

        RecipeResponse? recipeFromGetByID = await _recipeService.GetRecipeByID(invalidRecipeID);

        Assert.Null(recipeFromGetByID);
    }

    [Fact]
    public async Task GetRecipeByID_ValidID_RecipeFound()
    {
        Guid? validRecipeID = Guid.NewGuid();
        Recipe validRecipe = _fixture.Build<Recipe>()
            .With(x => x.ID, validRecipeID.Value)
            .Create();

        _recipeRepositoryMock.Setup(r => r.GetRecipeByID(It.IsAny<Guid>()))
            .ReturnsAsync(validRecipe);

        RecipeResponse? recipeFromGetByID = await _recipeService.GetRecipeByID(validRecipeID);

        Assert.NotNull(recipeFromGetByID);
        Assert.Equal(validRecipe.ToRecipeResponse(), recipeFromGetByID);
    }

    #endregion
    #region GetAllRecipes()

    [Fact]
    public async Task GetAllRecipes_RecipesExists_ShouldReturnCollection()
    {
        // Arrange
        List<Recipe> recipesInRepo = new()
        {
            _fixture.Create<Recipe>(),
            _fixture.Create<Recipe>(),
            _fixture.Create<Recipe>()
        };

        _recipeRepositoryMock
            .Setup(r => r.GetAllRecipes())
            .ReturnsAsync(recipesInRepo);

        List<RecipeResponse> recipesInRepoAsResponse = recipesInRepo.Select(r => r.ToRecipeResponse()).ToList();

        // Act
        var allRecipes = await _recipeService.GetAllRecipes();

        //Assert
        Assert.Equal(recipesInRepoAsResponse.Count, allRecipes.Count);
    }
    #endregion
    #region GetFilteredRecipes()
    [Fact]
    public async Task GetFilteredRecipes_ProperSearchValues_ReturnFilteredRecipes()
    {
        List<Recipe> fakeList = new()
        {
            _fixture.Build<Recipe>().With(r => r.Name, "spaghetti").Create(),
            _fixture.Build<Recipe>().With(r => r.Name, "gh").Create(),
            _fixture.Create<Recipe>()

        };

        _recipeRepositoryMock
            .Setup(r => r.GetFilteredRecipes(It.IsAny<Expression<Func<Recipe, bool>>>()))
            .Returns(async (Expression<Func<Recipe, bool>> filter) =>
            {
                var data = fakeList.AsQueryable().Where(filter).ToList();
                return await Task.FromResult(data);
            });

        string? searchString = "Spaghetti";
        string? searchBy = nameof(RecipeResponse.Name);

        List<RecipeResponse> expectedFilteredRecipes = fakeList
            .Select(r => r.ToRecipeResponse())
            .Where(r =>
                !string.IsNullOrWhiteSpace(r.Name)
                && r.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            .ToList();

        List<RecipeResponse> filteredRecipes = await _recipeService.GetFilteredRecipes(searchBy, searchString);


        // Assert
        Assert.Equal(expectedFilteredRecipes.Count, filteredRecipes.Count);
        Assert.NotEmpty(filteredRecipes);
        Assert.All(filteredRecipes, r =>
        Assert.Contains(searchString, r.Name, StringComparison.OrdinalIgnoreCase));
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetFilteredRecipes_NullOrEmptySearchName_ReturnAllRecipes(string searchString)
    {
        List<Recipe> fakeList = _fixture.CreateMany<Recipe>(5).ToList();

        _recipeRepositoryMock.Setup(r => r.GetAllRecipes()).ReturnsAsync(fakeList);
        _recipeRepositoryMock
            .Setup(r => r.GetFilteredRecipes(It.IsAny<Expression<Func<Recipe, bool>>>()))
            .ReturnsAsync(fakeList);

        string? searchBy = nameof(RecipeResponse.Name);
        List<RecipeResponse> filteredRecipes = await _recipeService.GetFilteredRecipes(searchBy, searchString);

        List<RecipeResponse> expectedFilteredRecipes = fakeList.Select(r => r.ToRecipeResponse()).ToList();
        Assert.Equal(expectedFilteredRecipes, filteredRecipes);
    }

    [Fact]
    public async Task GetFilteredRecipes_NullSearchBy_ReturnsAllRecipes()
    {
        // Arrange
        List<Recipe> fakeList = new()
        {
            _fixture.Create<Recipe>(),
            _fixture.Create<Recipe>(),
            _fixture.Build<Recipe>().With(r => r.Name,"sałatka").Create(),
            _fixture.Build<Recipe>().With(r => r.Name,"sałatka").Create()
        };
        _recipeRepositoryMock.Setup(r => r.GetAllRecipes()).ReturnsAsync(fakeList);

        // Act
        string? searchString = "sałatka";
        string? searchBy = null;

        List<RecipeResponse> filteredRecipes = await _recipeService.GetFilteredRecipes(searchBy, searchString);

        // Assert
        List<RecipeResponse> fakeListAsResponse = fakeList.Select(r => r.ToRecipeResponse()).ToList();
        Assert.Equal(fakeListAsResponse.Count, filteredRecipes.Count);
    }

    [Fact]
    public async Task GetFilteredRecipes_NoResults_ReturnsEmptyCollection()
    {
        List<Recipe> fakeList = _fixture.CreateMany<Recipe>(5).ToList();
        _recipeRepositoryMock
            .Setup(r => r.GetFilteredRecipes(It.IsAny<Expression<Func<Recipe, bool>>>()))
            .Returns(async (Expression<Func<Recipe, bool>> filter) =>
            {
                var data = fakeList.AsQueryable().Where(filter).ToList();
                return await Task.FromResult(data);
            });

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
        List<Recipe> fakeList = _fixture.CreateMany<Recipe>(5).ToList();
        _recipeRepositoryMock.Setup(r => r.GetAllRecipes()).ReturnsAsync(fakeList);

        List<RecipeResponse> fakeListAsResponse = fakeList.Select(r => r.ToRecipeResponse()).ToList();

        List<RecipeResponse>? sortedRecipes = await _recipeService.GetSortedRecipes(fakeListAsResponse, nameof(RecipeResponse.Author), true);

        var expectedOrder = fakeListAsResponse.OrderBy(r => r.Author).ToList();
        Assert.Equal(expectedOrder.Select(r => r.Author), sortedRecipes.Select(r => r.Author));
    }

    [Fact]
    public async Task GetSortedRecipes_ReturnDescendingOrder()
    {
        List<Recipe> fakeList = _fixture.CreateMany<Recipe>(5).ToList();
        _recipeRepositoryMock.Setup(r => r.GetAllRecipes()).ReturnsAsync(fakeList);

        List<RecipeResponse> fakeListAsResponse = fakeList.Select(r => r.ToRecipeResponse()).ToList();
        List<RecipeResponse>? sortedRecipes = await _recipeService.GetSortedRecipes(fakeListAsResponse, nameof(RecipeResponse.Author), false);

        var expectedOrder = fakeListAsResponse.OrderByDescending(r => r.Author).ToList();
        Assert.Equal(expectedOrder.Select(r => r.Author), sortedRecipes.Select(r => r.Author));
    }

    [Fact]
    public async Task GetSortedRecipes_CheckCount()
    {
        List<Recipe> fakeList = _fixture.CreateMany<Recipe>(5).ToList();
        _recipeRepositoryMock.Setup(r => r.GetAllRecipes()).ReturnsAsync(fakeList);

        List<RecipeResponse> fakeListAsResponse = fakeList.Select(r => r.ToRecipeResponse()).ToList();
        List<RecipeResponse>? sortedRecipes = await _recipeService.GetSortedRecipes(fakeListAsResponse, nameof(RecipeResponse.Author), false);

        Assert.Equal(sortedRecipes.Count, fakeListAsResponse.Count);
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
        List<Recipe> fakeList = _fixture.CreateMany<Recipe>(1).ToList();
        _recipeRepositoryMock.Setup(r => r.GetAllRecipes()).ReturnsAsync(fakeList);

        List<RecipeResponse> fakeListAsResponse = fakeList.Select(r => r.ToRecipeResponse()).ToList();

        List<RecipeResponse>? result = await _recipeService.GetSortedRecipes(fakeListAsResponse, nameof(RecipeResponse.Author), true);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetSortedRecipes_InvalidProperty_ReturnUnsorted()
    {
        List<Recipe> fakeList = _fixture.CreateMany<Recipe>(3).ToList();
        _recipeRepositoryMock.Setup(r => r.GetAllRecipes()).ReturnsAsync(fakeList);

        List<RecipeResponse> fakeListAsResponse = fakeList.Select(r => r.ToRecipeResponse()).ToList();

        List<RecipeResponse> sortedRecipes = await _recipeService.GetSortedRecipes(fakeListAsResponse, "abc", false);

        Assert.Equal(fakeListAsResponse.Count, sortedRecipes.Count);
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
        // Arrange
        List<Recipe> fakeDb = _fixture.CreateMany<Recipe>(3).ToList();
        Recipe testRecipe = fakeDb.Last();

        _recipeRepositoryMock
            .Setup(r => r.UpdateRecipe(It.IsAny<Recipe>()))
             .ReturnsAsync((Recipe r) => r);

        _recipeRepositoryMock
            .Setup(r => r.GetRecipeByID(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => fakeDb.FirstOrDefault(r => r.ID == id));

        RecipeUpdateRequest recipeToUpdate = _fixture.Build<RecipeUpdateRequest>()
            .With(x => x.ID, testRecipe.ID)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Create();

        // Act
        RecipeResponse updatedRecipe = await _recipeService.UpdateRecipe(recipeToUpdate);

        // Assert
        RecipeResponse testRecipeResponse = testRecipe.ToRecipeResponse();

        Assert.NotNull(updatedRecipe);
        Assert.Equal(testRecipeResponse.ID, updatedRecipe.ID);
        Assert.Equal(recipeToUpdate.Name, updatedRecipe.Name);
        Assert.Equal(recipeToUpdate.Description, updatedRecipe.Description);
        Assert.Equal(recipeToUpdate.Author, updatedRecipe.Author);
        Assert.Equal(recipeToUpdate.Category, updatedRecipe.Category);
        Assert.Equal(recipeToUpdate.PreparationTime, updatedRecipe.PreparationTime);
        Assert.Equal(recipeToUpdate.Servings, updatedRecipe.Servings);
        Assert.Equal(recipeToUpdate.Rating, updatedRecipe.Rating);
        Assert.Equal(recipeToUpdate.ImageUrl, updatedRecipe.ImageUrl);
        Assert.Equal(testRecipeResponse.CreatedAt, updatedRecipe.CreatedAt);
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