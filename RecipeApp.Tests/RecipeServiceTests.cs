using System.Linq.Expressions;
using AutoFixture;
using Moq;
using RecipeApp.Core.Domain.Entities;
using RecipeApp.Core.Domain.Enums;
using RecipeApp.Core.Domain.RepositoriesContracts;
using RecipeApp.Core.DTOs.RecipeDTO;
using RecipeApp.Core.DTOs.RecipeIngredientDTO;
using RecipeApp.Core.Helpers;
using RecipeApp.Core.Services;
using RecipeApp.Core.Services.RecipeServices;
using RecipeApp.Core.ServicesContracts;
using RecipeApp.Core.ServicesContracts.RecipeContracts;

namespace RecipeApp.Tests;

public class RecipeServiceTests
{
    private readonly IRecipeService _recipeService;

    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;

    private readonly IIngredientService _ingredientService;
    private readonly IRecipeIngredientService _recipeIngredientService;
    private readonly IFixture _fixture;
    private readonly IRecipeRepository _recipeRepository;
    private readonly Mock<IIngredientRepository> _ingredientRepositoryMock;
    private readonly IIngredientRepository _ingredientRepository;


    public RecipeServiceTests()
    {
        #region Fixture config
        _fixture = new Fixture();
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        #endregion


        _recipeRepositoryMock = new Mock<IRecipeRepository>();
        _ingredientRepositoryMock = new Mock<IIngredientRepository>();

        _recipeService = new RecipeService
            (
                new RecipeCommandService(_recipeRepositoryMock.Object),
                new RecipeDeleteService(_recipeRepositoryMock.Object),
                new RecipeFilterService(_recipeRepositoryMock.Object),
                new RecipeQueryService(_recipeRepositoryMock.Object)
            );
        _ingredientService = new IngredientService(_ingredientRepositoryMock.Object);
        _recipeIngredientService = new RecipeIngredientService(_recipeRepositoryMock.Object, _ingredientRepositoryMock.Object, _ingredientService);
    }

    private static void AssertSuccessHelper<T>(Result<T> result)
    {
        Assert.IsType<Result<T>>(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
    private static void AssertSuccessHelper(Result result)
    {
        Assert.IsType<Result>(result);
        Assert.True(result.IsSuccess);
    }

    private static void AssertFailureHelper<T>(Result<T> result)
    {
        Assert.IsType<Result<T>>(result);
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }
    private static void AssertFailureHelper(Result result)
    {
        Assert.IsType<Result>(result);
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    #region RequireIDOrStringValidator
    [Fact]
    public async Task RequireIDOrStringValidator_NullIngredientID()
    {
        var obj = _fixture.Build<RecipeIngredientAddRequest>()
            .With(ing => ing.IngredientID, Guid.Empty)
            .Create();
        bool result = ValidationHelper.ValidateModel(obj);
        Assert.True(result);
    }
    [Fact]
    public async Task RequireIDOrStringValidator_NullIngredientName()
    {
        var obj = _fixture.Build<RecipeIngredientAddRequest>()
            .With(ing => ing.IngredientName, String.Empty)
            .Create();
        bool result = ValidationHelper.ValidateModel(obj);
        Assert.True(result);
    }
    [Fact]
    public async Task RequireIDOrStringValidator_BothValuesExists()
    {
        var obj = _fixture.Build<RecipeIngredientAddRequest>()
            .Create();
        bool result = ValidationHelper.ValidateModel(obj);
        Assert.False(result);
    }
    [Fact]
    public async Task RequireIDOrStringValidator_BothValuesAreNull()
    {
        var obj = _fixture.Build<RecipeIngredientAddRequest>()
            .With(ing => ing.IngredientName, String.Empty)
            .With(ing => ing.IngredientID, Guid.Empty)
            .Create();
        bool result = ValidationHelper.ValidateModel(obj);
        Assert.False(result);
    }
    #endregion


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
        var emptyRequest = new RecipeAddRequest();

        var result = await _recipeService.AddRecipe(emptyRequest);

        Assert.False(result.IsSuccess);
        Assert.IsType<Result<RecipeResponse>>(result);
    }

    [Fact]
    public async Task AddRecipe_ProperRecipeDetails()
    {
        var validRequest = _fixture.Create<RecipeAddRequest>();

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

        var result = await _recipeService.AddRecipe(validRequest);

        AssertSuccessHelper(result);
        Assert.True(result.Value.ID != Guid.Empty);
    }

    [Fact]
    public async Task AddRecipe_MinimalProperRecipeDetails()
    {
        RecipeAddRequest validRequest = _fixture.Build<RecipeAddRequest>()
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

        var result = await _recipeService.AddRecipe(validRequest);

        AssertSuccessHelper(result);
        Assert.True(result.Value.ID != Guid.Empty);
    }
    #endregion
    #region GetRecipeByID()
    [Fact]
    public async Task GetRecipeByID_NullID()
    {
        Guid? nullID = null;

        _recipeRepositoryMock.Setup(r => r.GetRecipeByID(It.IsAny<Guid>()))
            .ReturnsAsync((Recipe?)null);

        var result = await _recipeService.GetRecipeByID(nullID);

        Assert.IsType<Result<RecipeResponse>>(result);
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task GetRecipeByID_ValidID_RecipeNotFound()
    {
        Guid? invalidID = Guid.NewGuid();

        _recipeRepositoryMock.Setup(r => r.GetRecipeByID(It.IsAny<Guid>()))
            .ReturnsAsync((Recipe?)null);

        var result = await _recipeService.GetRecipeByID(invalidID);

        AssertFailureHelper(result);
    }

    [Fact]
    public async Task GetRecipeByID_ValidID_RecipeFound()
    {
        Guid? validID = Guid.NewGuid();
        Recipe validRecipe = _fixture.Build<Recipe>()
            .With(x => x.ID, validID.Value)
            .Create();

        _recipeRepositoryMock.Setup(r => r.GetRecipeByID(It.IsAny<Guid>()))
            .ReturnsAsync(validRecipe);

        var result = await _recipeService.GetRecipeByID(validID);

        AssertSuccessHelper(result);
    }

    #endregion
    #region GetAllRecipes()

    [Fact]
    public async Task GetAllRecipes_RecipesExists_ShouldReturnCollection()
    {
        // Arrange
        List<Recipe> repoRecipes =
        [
            _fixture.Create<Recipe>(),
            _fixture.Create<Recipe>(),
            _fixture.Create<Recipe>()
        ];

        _recipeRepositoryMock
            .Setup(r => r.GetAllRecipes())
            .ReturnsAsync(repoRecipes);

        var repoRecipesAsResponse = repoRecipes.Select(r => r.ToRecipeResponse()).ToList();

        // Act
        var result = await _recipeService.GetAllRecipes();

        //Assert
        AssertSuccessHelper(result);
        Assert.Equal(repoRecipesAsResponse.Count, result.Value.Count);
    }
    #endregion
    #region GetFilteredRecipes()
    [Fact]
    public async Task GetFilteredRecipes_ProperSearchValues_ReturnFilteredRecipes()
    {
        List<Recipe> repoRecipes = new()
        {
            _fixture.Build<Recipe>().With(r => r.Name, "spaghetti").Create(),
            _fixture.Build<Recipe>().With(r => r.Name, "gh").Create(),
            _fixture.Create<Recipe>()

        };

        _recipeRepositoryMock
            .Setup(r => r.GetFilteredRecipes(It.IsAny<Expression<Func<Recipe, bool>>>()))
            .Returns(async (Expression<Func<Recipe, bool>> filter) =>
            {
                var data = repoRecipes.AsQueryable().Where(filter).ToList();
                return await Task.FromResult(data);
            });

        string? searchString = "Spaghetti";
        string? searchBy = nameof(RecipeResponse.Name);

        List<RecipeResponse> expectedFilteredRecipes = repoRecipes
            .Select(r => r.ToRecipeResponse())
            .Where(r =>
                !string.IsNullOrWhiteSpace(r.Name)
                && r.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var result = await _recipeService.GetFilteredRecipes(searchBy, searchString);


        // Assert
        AssertSuccessHelper(result);
        Assert.Equal(expectedFilteredRecipes, result.Value);
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetFilteredRecipes_NullOrEmptySearchName_ReturnAllRecipes(string? searchString)
    {
        List<Recipe> repoRecipes = _fixture.CreateMany<Recipe>(5).ToList();

        _recipeRepositoryMock.Setup(r => r.GetAllRecipes()).ReturnsAsync(repoRecipes);
        _recipeRepositoryMock
            .Setup(r => r.GetFilteredRecipes(It.IsAny<Expression<Func<Recipe, bool>>>()))
            .ReturnsAsync(repoRecipes);

        string? searchBy = nameof(RecipeResponse.Name);
        var result = await _recipeService.GetFilteredRecipes(searchBy, searchString);

        // Assert
        List<RecipeResponse> expected = repoRecipes.Select(r => r.ToRecipeResponse()).ToList();
        AssertSuccessHelper(result);
        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task GetFilteredRecipes_NullSearchBy_ReturnsAllRecipes()
    {
        // Arrange
        List<Recipe> repoRecipes = new()
        {
            _fixture.Create<Recipe>(),
            _fixture.Create<Recipe>(),
            _fixture.Build<Recipe>().With(r => r.Name,"sałatka").Create(),
            _fixture.Build<Recipe>().With(r => r.Name,"sałatka").Create()
        };
        _recipeRepositoryMock.Setup(r => r.GetAllRecipes()).ReturnsAsync(repoRecipes);

        // Act
        string? searchString = "sałatka";
        string? searchBy = null;

        var result = await _recipeService.GetFilteredRecipes(searchBy, searchString);

        // Assert
        List<RecipeResponse> expected = repoRecipes.Select(r => r.ToRecipeResponse()).ToList();
        AssertSuccessHelper(result);
        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task GetFilteredRecipes_NoResults_ReturnsEmptyCollection()
    {
        List<Recipe> repoRecipes = _fixture.CreateMany<Recipe>(5).ToList();
        _recipeRepositoryMock
            .Setup(r => r.GetFilteredRecipes(It.IsAny<Expression<Func<Recipe, bool>>>()))
            .Returns(async (Expression<Func<Recipe, bool>> filter) =>
            {
                var data = repoRecipes.AsQueryable().Where(filter).ToList();
                return await Task.FromResult(data);
            });

        string? searchString = "Paella z kurczakiem";
        string? searchBy = nameof(RecipeResponse.Name);
        var result = await _recipeService.GetFilteredRecipes(searchBy, searchString);

        AssertSuccessHelper(result);
        Assert.Empty(result.Value);
    }

    #endregion
    #region GetSortedRecipes()
    [Fact]
    public async Task GetSortedRecipes_ReturnAscendingOrder()
    {
        List<Recipe> repoRecipes = _fixture.CreateMany<Recipe>(5).ToList();
        _recipeRepositoryMock.Setup(r => r.GetAllRecipes()).ReturnsAsync(repoRecipes);

        List<RecipeResponse> repoRecipesAsResponse = repoRecipes.Select(r => r.ToRecipeResponse()).ToList();

        var result = await _recipeService.GetSortedRecipes(repoRecipesAsResponse, nameof(RecipeResponse.Author), true);

        var expected = repoRecipesAsResponse.OrderBy(r => r.Author).ToList();

        AssertSuccessHelper(result);
        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task GetSortedRecipes_ReturnDescendingOrder()
    {
        List<Recipe> repoRecipes = _fixture.CreateMany<Recipe>(5).ToList();
        _recipeRepositoryMock.Setup(r => r.GetAllRecipes()).ReturnsAsync(repoRecipes);

        List<RecipeResponse> repoRecipesAsResponse = repoRecipes.Select(r => r.ToRecipeResponse()).ToList();
        var result = await _recipeService.GetSortedRecipes(repoRecipesAsResponse, nameof(RecipeResponse.Author), false);

        var expected = repoRecipesAsResponse.OrderByDescending(r => r.Author).ToList();
        AssertSuccessHelper(result);
        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task GetSortedRecipes_CheckCount()
    {
        List<Recipe> repoRecipes = _fixture.CreateMany<Recipe>(5).ToList();
        _recipeRepositoryMock.Setup(r => r.GetAllRecipes()).ReturnsAsync(repoRecipes);

        List<RecipeResponse> repoRecipesAsResponse = repoRecipes.Select(r => r.ToRecipeResponse()).ToList();
        var result = await _recipeService.GetSortedRecipes(repoRecipesAsResponse, nameof(RecipeResponse.Author), false);

        AssertSuccessHelper(result);
        Assert.Equal(repoRecipesAsResponse.Count, result.Value.Count);
    }

    [Fact]
    public async Task GetSortedRecipes_EmptyList_ReturnEmpty()
    {
        var result = await _recipeService.GetSortedRecipes(new List<RecipeResponse>(), nameof(RecipeResponse.Author), false);

        AssertSuccessHelper(result);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetSortedRecipes_SingleElement_ReturnsSameList()
    {
        List<Recipe> repoRecipes = _fixture.CreateMany<Recipe>(1).ToList();
        _recipeRepositoryMock.Setup(r => r.GetAllRecipes()).ReturnsAsync(repoRecipes);

        List<RecipeResponse> repoRecipesAsResponse = repoRecipes.Select(r => r.ToRecipeResponse()).ToList();

        var result = await _recipeService.GetSortedRecipes(repoRecipesAsResponse, nameof(RecipeResponse.Author), true);

        AssertSuccessHelper(result);
        Assert.Single(result.Value);
    }

    [Fact]
    public async Task GetSortedRecipes_InvalidProperty_ReturnUnsorted()
    {
        List<Recipe> repoRecipes = _fixture.CreateMany<Recipe>(3).ToList();
        _recipeRepositoryMock.Setup(r => r.GetAllRecipes()).ReturnsAsync(repoRecipes);

        List<RecipeResponse> expected = repoRecipes.Select(r => r.ToRecipeResponse()).ToList();

        var result = await _recipeService.GetSortedRecipes(expected, "abc", false);

        AssertSuccessHelper(result);
        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task GetSortedRecipes_NullList_ReturnsResultFailure()
    {
        var result = await _recipeService.GetSortedRecipes(null, nameof(RecipeResponse.Author), true);

        AssertFailureHelper(result);
    }
    #endregion
    #region UpdateRecipe()
    [Fact]
    public async Task UpdateRecipe_NullRecipe()
    {
        RecipeUpdateRequest? request = null;
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _recipeService.UpdateRecipe(request);
        });
    }

    [Fact]
    public async Task UpdateRecipe_NullID()
    {
        RecipeUpdateRequest recipeUpdateRequest = _fixture.Build<RecipeUpdateRequest>()
            .With(x => x.ID, Guid.Empty)
            .Create();

        var result = await _recipeService.UpdateRecipe(recipeUpdateRequest);

        AssertFailureHelper(result);
    }

    [Fact]
    public async Task UpdateRecipe_ValidData_UpdatesSuccessfully()
    {
        // Arrange
        List<Recipe> repoRecipes = _fixture.CreateMany<Recipe>(3).ToList();
        Recipe testRecipe = repoRecipes.Last();

        _recipeRepositoryMock
            .Setup(r => r.UpdateRecipe(It.IsAny<Recipe>()))
             .ReturnsAsync((Recipe r) => r);

        _recipeRepositoryMock
            .Setup(r => r.GetRecipeByID(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => repoRecipes.FirstOrDefault(r => r.ID == id));

        RecipeUpdateRequest recipeToUpdate = _fixture.Build<RecipeUpdateRequest>()
            .With(x => x.ID, testRecipe.ID)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .Create();

        // Act
        var result = await _recipeService.UpdateRecipe(recipeToUpdate);

        // Assert
        RecipeResponse expected = testRecipe.ToRecipeResponse();

        AssertSuccessHelper(result);
        Assert.Equal(expected.ID, result.Value.ID);
        Assert.Equal(recipeToUpdate.Name, result.Value.Name);
        Assert.Equal(recipeToUpdate.Description, result.Value.Description);
        Assert.Equal(recipeToUpdate.Author, result.Value.Author);
        Assert.Equal(recipeToUpdate.Category, result.Value.Category);
        Assert.Equal(recipeToUpdate.PreparationTime, result.Value.PreparationTime);
        Assert.Equal(recipeToUpdate.Servings, result.Value.Servings);
        Assert.Equal(recipeToUpdate.Rating, result.Value.Rating);
        Assert.Equal(recipeToUpdate.ImageUrl, result.Value.ImageUrl);
        Assert.Equal(expected.CreatedAt, result.Value.CreatedAt);
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
        var result = await _recipeService.DeleteRecipe(invalidRecipeID);

        AssertFailureHelper(result);
    }

    public async Task DeleteRecipe_ValidID_DeletesSuccessfully()
    {
        List<Recipe> repoRecipes = _fixture.CreateMany<Recipe>(2).ToList();
        _recipeRepositoryMock
            .Setup(r => r.GetRecipeByID(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => repoRecipes.FirstOrDefault(r => r.ID == id));
        _recipeRepositoryMock
            .Setup(r => r.DeleteRecipe(It.IsAny<Guid>()))
            .Returns((Guid id) =>
            {
                var recipe = repoRecipes.FirstOrDefault(r => r.ID == id);
                bool removed = false;
                if (recipe is not null)
                {
                    repoRecipes.Remove(recipe);
                    removed = true;
                }
                return Task.FromResult(removed);
            });

        Recipe recipeToDelete = repoRecipes.First();
        Guid? validRecipeID = recipeToDelete.ID;

        var result = await _recipeService.DeleteRecipe(validRecipeID);

        AssertSuccessHelper(result);
    }
    #endregion

    #region AddRecipeIngredient()
    [Fact]
    public async Task AddRecipeIngredient_ValidRequest()
    {
        Recipe dbRecipe = _fixture.Create<Recipe>();
        _recipeRepositoryMock.Setup(r => r.GetRecipeByID(It.IsAny<Guid>()))
            .ReturnsAsync(dbRecipe);
        RecipeIngredient dbRI = _fixture.Build<RecipeIngredient>()
            .With(i => i.RecipeID, dbRecipe.ID)
            .Create();
        _recipeRepositoryMock.Setup(r => r.InsertRecipeIngredient(It.IsAny<RecipeIngredient>()))
            .Callback<RecipeIngredient>(dbRI =>
            {
                dbRecipe.RecipeIngredients ??= new List<RecipeIngredient>();
                dbRecipe.RecipeIngredients.Add(dbRI);
            })
            .ReturnsAsync(dbRecipe);
        Ingredient dbIng = _fixture.Build<Ingredient>()
            .With(i => i.ID, dbRI.IngredientID)
            .Create();
        _ingredientRepositoryMock.Setup(r => r.GetIngredientByID(It.IsAny<Guid>()))
            .ReturnsAsync(dbIng);

        RecipeIngredientAddRequest request = _fixture.Build<RecipeIngredientAddRequest>()
            .With(ri => ri.IngredientName, String.Empty)
            .With(ri => ri.IngredientID, dbRI.IngredientID)
            .With(ri => ri.RecipeID, dbRecipe.ID)
            .With(ri => ri.Unit, dbRI.Unit)
            .With(ri => ri.Quantity, dbRI.Quantity)
            .Create();

        var result = await _recipeIngredientService.AddRecipeIngredient(request);

        AssertSuccessHelper(result);
    }

    [Fact]
    public async Task AddRecipeIngredient_InvalidQuantity_ShouldFailValidation()
    {
        RecipeIngredientAddRequest request = _fixture.Build<RecipeIngredientAddRequest>()
            .With(x => x.Quantity, 0)
            .Create();

        var result = await _recipeIngredientService.AddRecipeIngredient(request);

        AssertFailureHelper(result);
    }

    [Fact]
    public async Task AddRecipeIngredient_InvalidIngredientValues_ReturnsFailure()
    {
        // Arrange
        RecipeIngredientAddRequest request = _fixture.Build<RecipeIngredientAddRequest>()
            .With(x => x.IngredientID, Guid.Empty)
            .With(x => x.IngredientName, String.Empty)
            .Create();

        // Act
        var result = await _recipeIngredientService.AddRecipeIngredient(request);

        AssertFailureHelper(result);
    }

    [Fact]
    public async Task AddRecipeIngredient_EmptyRecipeID_ReturnsFailure()
    {
        RecipeIngredientAddRequest request = _fixture.Build<RecipeIngredientAddRequest>()
            .With(x => x.RecipeID, Guid.Empty)
            .Create();

        var result = await _recipeIngredientService.AddRecipeIngredient(request);

        AssertFailureHelper(result);
    }

    [Fact]
    public async Task AddRecipeIngredient_MissingUnit_ShouldFailValidation()
    {
        RecipeIngredientAddRequest request = _fixture.Build<RecipeIngredientAddRequest>()
            .With(x => x.Unit, (Unit?)null)
            .Create();

        var result = await _recipeIngredientService.AddRecipeIngredient(request);

        AssertFailureHelper(result);
    }

    [Fact]
    public async Task AddRecipeIngredient_AlreadyExists_ReturnsError()
    {
        RecipeIngredientAddRequest requestRI = _fixture.Create<RecipeIngredientAddRequest>();
        RecipeIngredient dbRI = requestRI.ToRecipeIngredient();
        dbRI.ID = _fixture.Create<Guid>();
        Recipe dbRecipe = _fixture.Build<Recipe>()
            .With(r => r.RecipeIngredients, new List<RecipeIngredient>() { dbRI })
            .Create();


        var result = await _recipeIngredientService.AddRecipeIngredient(requestRI);

        AssertFailureHelper(result);
    }

    #endregion
    /*
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

        var recipeWithIngredient = await _recipeIngredientService.AddRecipeIngredient(recipeAddIngredientRequest);

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

    //[Fact]
    //public async Task DeleteRecipeIngredient_InvalidID_ReturnsFalse()
    //{
    //    Guid? invalidRecipeIngredientID = Guid.NewGuid();
    //    bool result = await _recipeService.DeleteRecipe(invalidRecipeIngredientID);
    //    Assert.False(result);
    //}

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
    #endregion
    */
}
