using AutoFixture;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Application.DTOs.IngredientDTO;
using RecipeApp.Application.Interfaces;
using RecipeApp.Application.Services;
using RecipeApp.Domain.Entities;
using RecipeApp.Infrastructure;

namespace RecipeApp.Tests;

public class IngredientServiceTests
{
    private readonly IIngredientService _ingredientService;
    private readonly IFixture _fixture;
    private readonly ApplicationDbContext _dbContext;

    public IngredientServiceTests()
    {
        List<Ingredient>? ingredientsInitialData = new List<Ingredient>() { };
        _fixture = new Fixture();

        DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options
            );

        _dbContext = dbContextMock.Object;
        dbContextMock.CreateDbSetMock(temp => temp.Ingredients, ingredientsInitialData);

        _ingredientService = new IngredientService(_dbContext);
    }

    #region AddIngredient()
    [Fact]
    public async Task AddIngredient_ValidIngredient_ShouldReturnIngredientResponse()
    {
        IngredientAddRequest ingredientToAdd = _fixture.Create<IngredientAddRequest>();

        IngredientResponse? ingredientResponseFromAdd = await _ingredientService.AddIngredient(ingredientToAdd);

        Assert.NotNull(ingredientResponseFromAdd);
        Assert.Equal(ingredientToAdd.Name, ingredientResponseFromAdd.Name);
        Assert.NotEqual(ingredientResponseFromAdd.ID, Guid.Empty);
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

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _ingredientService.AddIngredient(ingredientToAdd);
        });
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
        Guid? ingredientID = null;
        IngredientResponse? ingredientFromGetByID = await _ingredientService.GetIngredientByID(ingredientID);

        Assert.Null(ingredientFromGetByID);
    }

    [Fact]
    public async Task GetIngredientByID_ValidID_IngredientNotFound()
    {
        Guid? ingredientID = Guid.NewGuid();
        IngredientResponse? ingredientFromGetByID = await _ingredientService.GetIngredientByID(ingredientID);

        Assert.Null(ingredientFromGetByID);
    }

    [Fact]
    public async Task GetIngredientByID_ValidID_IngredientFound()
    {
        Ingredient ingredient = _fixture.Create<Ingredient>();
        _dbContext.Ingredients.Add(ingredient);
        await _dbContext.SaveChangesAsync();

        IngredientResponse? ingredientFromGetByID = await _ingredientService.GetIngredientByID(ingredient.ID);

        Assert.NotNull(ingredientFromGetByID);
        Assert.Equal(ingredient.ID, ingredientFromGetByID.ID);
    }


    #endregion
    #region GetAllIngredients()
    [Fact]
    public async Task GetAllIngredients_EmptyList()
    {
        List<IngredientResponse>? ingredientsFromGetAll = await _ingredientService.GetAllIngredients();

        Assert.Empty(ingredientsFromGetAll);
        Assert.NotNull(ingredientsFromGetAll);
    }

    [Fact]
    public async Task GetAllIngredients_AddMultipleIngredients_ShouldContainThemAll()
    {
        List<Ingredient>? ingredients = _fixture.CreateMany<Ingredient>(3).ToList();
        ingredients.Select(i => _dbContext.Ingredients.Add(i));
        await _dbContext.SaveChangesAsync();

        // Pobranie wszystkich przepisów
        List<IngredientResponse>? allIngredients = await _ingredientService.GetAllIngredients();

        // Sprawdzenie, czy wszystkie dodane przepisy znajdują się w kolekcji
        foreach (Ingredient ingredient in ingredients)
        {
            Assert.Contains(allIngredients, r => r.ID == ingredient.ID);
        }
    }

    #endregion
    #region GetFilteredIngredients()
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetFilteredIngredients_InvalidSearchString(string? searchString)
    {
        List<Ingredient>? ingredients = _fixture.CreateMany<Ingredient>(3).ToList();
        ingredients.Select(i => _dbContext.Ingredients.Add(i));
        await _dbContext.SaveChangesAsync();

        List<IngredientResponse>? filteredIngredientsList = await _ingredientService.GetFilteredIngredients(searchString);

        Assert.Equal(ingredients!.Count, filteredIngredientsList!.Count);
    }

    [Fact]
    public async Task GetFilteredIngredients_ValidSearchString_ReturnFilteredIngredients()
    {
        // Arrange
        string? searchString = "Po";

        var names = new[] { "Rzodkiewka", "Ogórek", "Pomidor" };

        var ingredientList = names
            .Select(name => _fixture.Build<Ingredient>()
                .With(i => i.Name, name)
                .Create())
            .ToList();

        ingredientList.ForEach(i => _dbContext.Ingredients.Add(i));
        List<IngredientResponse> ingredientResponseList = ingredientList.Select(i => i.ToIngredientResponse()).ToList();
        await _dbContext.SaveChangesAsync();

        // Act
        List<IngredientResponse>? filteredIngredientsList = await _ingredientService.GetFilteredIngredients(searchString);
        List<IngredientResponse>? expectedIngredientsList = ingredientResponseList
            .Where(temp => temp.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Assert
        Assert.Equal(expectedIngredientsList, ingredientResponseList);
        Assert.Equal(expectedIngredientsList!.Count, filteredIngredientsList!.Count);
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
            .With(x => x.ID, (Guid?)null)
            .Create();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _ingredientService.UpdateIngredient(ingredientUpdateRequest);
        });
    }

    [Fact]
    public async Task UpdateIngredient_ValidData_UpdatesSuccessfully()
    {
        Ingredient ingredient = _fixture.Build<Ingredient>()
            .With(x => x.Name, "Pomidor")
            .Create();
        _dbContext.Ingredients.Add(ingredient);
        await _dbContext.SaveChangesAsync();

        IngredientUpdateRequest ingredientToUpdate = _fixture.Build<IngredientUpdateRequest>()
            .With(x => x.ID, ingredient.ID)
            .With(x => x.Name, "test")
            .Create();

        IngredientResponse? updatedIngredient = await _ingredientService.UpdateIngredient(ingredientToUpdate);

        // Assert
        Assert.NotNull(updatedIngredient);
        Assert.Equal(ingredient.ID, updatedIngredient.ID);
        Assert.Equal(ingredientToUpdate.Name, updatedIngredient.Name);
    }
    #endregion
    #region DeleteIngredient()
    [Fact]
    public async Task DeleteIngredient_NullID()
    {
        Guid? invalidIngredientID = null;
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _ingredientService.DeleteIngredient(invalidIngredientID);
        });
    }

    [Fact]
    public async Task DeleteIngredient_InvalidID_ReturnsFalse()
    {
        Guid? invalidIngredientID = Guid.NewGuid();
        bool result = await _ingredientService.DeleteIngredient(invalidIngredientID);
        Assert.False(result);
    }

    public async Task DeleteIngredient_ValidID_DeletesSuccessfully()
    {
        Ingredient addedIngredient = _fixture.Build<Ingredient>()
            .With(x => x.Name, "Pomidor")
            .Create();
        _dbContext.Ingredients.Add(addedIngredient);
        await _dbContext.SaveChangesAsync();

        bool deleteResult = await _ingredientService.DeleteIngredient(addedIngredient.ID);

        Ingredient? ingredientFromDb = await _dbContext.Ingredients.SingleOrDefaultAsync(i => i.ID == addedIngredient.ID);
        List<Ingredient>? allIngredients = await _dbContext.Ingredients.ToListAsync();

        Assert.True(deleteResult);
        Assert.Null(ingredientFromDb);
        Assert.DoesNotContain(addedIngredient, allIngredients);
    }
    #endregion
}
