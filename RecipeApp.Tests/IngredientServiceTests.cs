using System.Linq.Expressions;
using AutoFixture;
using Moq;
using RecipeApp.Application.DTOs.IngredientDTO;
using RecipeApp.Application.Helpers;
using RecipeApp.Application.Interfaces;
using RecipeApp.Application.Services;
using RecipeApp.Domain.Entities;
using RecipeApp.Domain.RepositoriesContracts;

namespace RecipeApp.Tests;

public class IngredientServiceTests
{
    private readonly IIngredientService _ingredientService;
    private readonly IFixture _fixture;
    private readonly Mock<IIngredientRepository> _ingredientRepositoryMock;
    private readonly IIngredientRepository _ingredientRepository;


    public IngredientServiceTests()
    {
        _ingredientRepositoryMock = new Mock<IIngredientRepository>();
        _ingredientRepository = _ingredientRepositoryMock.Object;

        _fixture = new Fixture();
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _ingredientService = new IngredientService(_ingredientRepository);
    }

    #region Helper methods
    private void AssertSuccessHelper<T>(Result<T> result)
    {
        Assert.IsType<Result<T>>(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
    private void AssertSuccessHelper(Result result)
    {
        Assert.IsType<Result>(result);
        Assert.True(result.IsSuccess);
    }

    private void AssertFailureHelper<T>(Result<T> result)
    {
        Assert.IsType<Result<T>>(result);
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }
    private void AssertFailureHelper(Result result)
    {
        Assert.IsType<Result>(result);
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }
    #endregion

    #region AddIngredient()
    [Fact]
    public async Task AddIngredient_ValidIngredient_ShouldReturnIngredientResponse()
    {
        IngredientAddRequest request = _fixture.Create<IngredientAddRequest>();

        var result = await _ingredientService.AddIngredient(request);

        AssertSuccessHelper(result);
        Assert.NotEqual(Guid.Empty, result.Value.ID);
        Assert.Equal(request.Name, result.Value.Name);
    }

    [Theory]
    [InlineData("")]       // za krótka
    [InlineData("A")]      // za krótka
    [InlineData(null)]     // null
    [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor")] // za długa
    public async Task AddIngredient_InvalidName_ShouldFailValidation(string? invalidName)
    {
        IngredientAddRequest ingredientToAdd = _fixture.Build<IngredientAddRequest>()
            .With(x => x.Name, invalidName)
            .Create();

        var result = await _ingredientService.AddIngredient(ingredientToAdd);

        AssertFailureHelper(result);
    }

    [Fact]
    public async Task AddIngredient_NullIngredient_ShouldFailValidation()
    {
        IngredientAddRequest? ingredientToAdd = null;

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _ingredientService.AddIngredient(ingredientToAdd);
        });
    }

    #endregion
    #region GetIngredientByID()
    public async Task GetRecipeByID_NullID()
    {
        Guid? nullID = null;
        var result = await _ingredientService.GetIngredientByID(nullID);

        AssertFailureHelper(result);
    }

    [Fact]
    public async Task GetIngredientByID_ValidID_IngredientNotFound()
    {
        Guid? invalidID = Guid.NewGuid();
        var result = await _ingredientService.GetIngredientByID(invalidID);

        AssertFailureHelper(result);
    }

    [Fact]
    public async Task GetIngredientByID_ValidID_IngredientFound()
    {
        Ingredient expectedIngredient = _fixture.Build<Ingredient>()
            .With(i => i.ID, Guid.NewGuid())
            .Create();

        _ingredientRepositoryMock.Setup(r => r.GetIngredientByID(It.IsAny<Guid>()))
            .ReturnsAsync(expectedIngredient);


        var result = await _ingredientService.GetIngredientByID(expectedIngredient.ID);

        AssertSuccessHelper(result);
        Assert.Equal(expectedIngredient.ID, result.Value.ID);
    }

    #endregion
    #region GetAllIngredients()
    [Fact]
    public async Task GetAllIngredients_EmptyList()
    {
        _ingredientRepositoryMock.Setup(r => r.GetAllIngredients())
            .ReturnsAsync(new List<Ingredient>());

        var result = await _ingredientService.GetAllIngredients();

        AssertSuccessHelper(result);
        Assert.Empty(result.Value);
        _ingredientRepositoryMock.Verify(r => r.GetAllIngredients(), Times.Once());
    }

    [Fact]
    public async Task GetAllIngredients_AddMultipleIngredients_ShouldContainThemAll()
    {
        List<Ingredient>? repoIngredients = _fixture.CreateMany<Ingredient>(3).ToList();
        _ingredientRepositoryMock.Setup(r => r.GetAllIngredients())
            .ReturnsAsync(repoIngredients);

        // Pobranie wszystkich przepisów
        var result = await _ingredientService.GetAllIngredients();

        List<IngredientResponse> expected = repoIngredients.Select(r => r.ToIngredientResponse()).ToList();
        // Sprawdzenie, czy wszystkie dodane przepisy znajdują się w kolekcji
        AssertSuccessHelper(result);
        Assert.Equal(expected, result.Value);
    }

    #endregion
    #region GetFilteredIngredients()
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetFilteredIngredients_InvalidSearchString(string? searchString)
    {
        List<Ingredient> repoIngredients = _fixture.CreateMany<Ingredient>(3).ToList();
        _ingredientRepositoryMock.Setup(i => i.GetAllIngredients()).ReturnsAsync(repoIngredients);

        var result = await _ingredientService.GetFilteredIngredients(searchString);

        List<IngredientResponse> expected = repoIngredients.Select(i => i.ToIngredientResponse()).ToList();
        AssertSuccessHelper(result);
        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task GetFilteredIngredients_ValidSearchString_ReturnFilteredIngredients()
    {
        // Arrange
        var names = new[] { "Rzodkiewka", "Ogórek", "Pomidor", };
        List<Ingredient> dbIngredients = names
            .Select(name => _fixture.Build<Ingredient>()
                .With(i => i.Name, name)
                .Create())
            .ToList();
        string? searchString = "rzodkiewka";

        _ingredientRepositoryMock.Setup(r => r.GetAllIngredients()).ReturnsAsync(dbIngredients);
        _ingredientRepositoryMock.Setup(r => r.GetFilteredIngredients(It.IsAny<Expression<Func<Ingredient, bool>>>()))
            .ReturnsAsync(dbIngredients.Where(i => i.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)).ToList());

        // Act
        var result = await _ingredientService.GetFilteredIngredients(searchString);
        var expected = dbIngredients.Select(i => i.ToIngredientResponse())
            .Where(temp => temp.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Assert
        AssertSuccessHelper(result);
        Assert.Equal(expected, result.Value);
    }
    #endregion
    #region UpdateIngredient()
    [Fact]
    public async Task UpdateIngredient_NullIngredient()
    {
        IngredientUpdateRequest? ingredientUpdateRequest = null;
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _ingredientService.UpdateIngredient(ingredientUpdateRequest);
        });
    }

    [Fact]
    public async Task UpdateIngredient_NullID()
    {
        IngredientUpdateRequest ingredientUpdateRequest = _fixture.Build<IngredientUpdateRequest>()
            .With(x => x.ID, Guid.Empty)
            .Create();

        var result = await _ingredientService.UpdateIngredient(ingredientUpdateRequest);

        AssertFailureHelper(result);
    }

    [Fact]
    public async Task UpdateIngredient_ValidData_UpdatesSuccessfully()
    {
        List<Ingredient> repoIngredients = _fixture.CreateMany<Ingredient>(3).ToList();
        Ingredient testIngredient = repoIngredients.First();
        IngredientUpdateRequest request = _fixture.Build<IngredientUpdateRequest>()
            .With(i => i.ID, testIngredient.ID)
            .Create();

        _ingredientRepositoryMock.Setup(r => r.GetIngredientByID(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => repoIngredients.FirstOrDefault(i => i.ID == id));

        var result = await _ingredientService.UpdateIngredient(request);

        // Assert
        AssertSuccessHelper(result);
        Assert.Equal(testIngredient.ID, result.Value.ID);
        Assert.Equal(testIngredient.Name, result.Value.Name);
    }
    #endregion
    #region DeleteIngredient()
    [Fact]
    public async Task DeleteIngredient_NullID()
    {
        Guid? invalidIngredientID = null;
        var result = await _ingredientService.DeleteIngredient(invalidIngredientID);

        AssertFailureHelper(result);

    }

    [Fact]
    public async Task DeleteIngredient_InvalidID_ReturnsFalse()
    {
        Guid? invalidIngredientID = Guid.NewGuid();
        var result = await _ingredientService.DeleteIngredient(invalidIngredientID);

        AssertFailureHelper(result);
        Assert.False(result.IsSuccess);
    }

    public async Task DeleteIngredient_ValidID_DeletesSuccessfully()
    {
        Ingredient addedIngredient = _fixture.Build<Ingredient>()
            .With(x => x.Name, "Pomidor")
            .Create();

        _ingredientRepositoryMock.Setup(r => r.DeleteIngredient(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        var result = await _ingredientService.DeleteIngredient(addedIngredient.ID);

        AssertSuccessHelper(result);
        _ingredientRepositoryMock.Verify(r => r.DeleteIngredient(addedIngredient.ID), Times.Once());
    }

    #endregion
}
