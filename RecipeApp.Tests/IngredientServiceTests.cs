using System.ComponentModel.DataAnnotations;
using RecipeApp.Application.DTOs.IngredientDTO;
using RecipeApp.Application.Interfaces;
using RecipeApp.Application.Services;

namespace RecipeApp.Tests;

public class IngredientServiceTests
{
    private readonly IIngredientService _ingredientService;

    public IngredientServiceTests()
    {
        _ingredientService = new IngredientService();
    }

    #region AddIngredient()
    [Fact]
    public void AddIngredient_ValidIngredient_ShouldReturnIngredientResponse()
    {
        IngredientAddRequest ingredientToAdd = new() { Name = "Pomidor" };

        IngredientResponse? ingredientResponseFromAdd = _ingredientService.AddIngredient(ingredientToAdd);

        Assert.NotNull(ingredientResponseFromAdd);
        Assert.Equal(ingredientToAdd.Name, ingredientResponseFromAdd.Name);
        Assert.NotEqual(ingredientResponseFromAdd.ID, Guid.Empty);
    }

    [Theory]
    [InlineData("")]       // za krótka
    [InlineData("A")]      // za krótka
    [InlineData(null)]     // null
    [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor")] // za długa
    public void AddIngredient_InvalidName_ShouldFailValidation(string? invalidName)
    {
        IngredientAddRequest ingredientToAdd = new() { Name = invalidName };

        IngredientResponse? ingredientResponseFromAdd = _ingredientService.AddIngredient(ingredientToAdd);

        var validationContext = new ValidationContext(ingredientResponseFromAdd);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(ingredientResponseFromAdd, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid); // Obiekt powinien być niepoprawny
        Assert.NotEmpty(validationResults); // Powinna być przynajmniej jedna wiadomość błędu
    }

    [Fact]
    public void AddIngredient_NullIngredient_ShouldFailValidation()
    {
        IngredientAddRequest? ingredientToAdd = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            IngredientResponse ingredientResponseFromAdd = _ingredientService.AddIngredient(ingredientToAdd);
        });
    }

    #endregion
    #region GetIngredientByID()
    public void GetRecipeByID_NullID()
    {
        Guid? ingredientID = null;
        IngredientResponse? ingredientFromGetByID = _ingredientService.GetIngredientByID(ingredientID);

        Assert.Null(ingredientFromGetByID);
    }

    [Fact]
    public void GetRecipeByID_ValidID_RecipeNotFound()
    {
        Guid? ingredientID = Guid.NewGuid();
        IngredientResponse? ingredientFromGetByID = _ingredientService.GetIngredientByID(ingredientID);

        Assert.Null(ingredientFromGetByID);
    }

    [Fact]
    public void GetRecipeByID_ValidID_RecipeFound()
    {
        IngredientAddRequest ingredientToAdd = new() { Name = "Pomidor" };
        IngredientResponse? ingredientResponseFromAdd = _ingredientService.AddIngredient(ingredientToAdd);
        Guid? validIngredientID = ingredientResponseFromAdd.ID;

        IngredientResponse? ingredientFromGetByID = _ingredientService.GetIngredientByID(validIngredientID);

        Assert.NotNull(ingredientFromGetByID);
        Assert.Equal(validIngredientID, ingredientFromGetByID.ID);
    }


    #endregion
    #region GetAllIngredients()
    [Fact]
    public void GetAllIngredients_EmptyList()
    {
        List<IngredientResponse>? ingredientsFromGetAll = _ingredientService.GetAllIngredients();

        Assert.Empty(ingredientsFromGetAll);
        Assert.NotNull(ingredientsFromGetAll);
    }

    [Fact]
    public void GetAllIngredients_AddMultipleIngredients_ShouldContainThemAll()
    {
        IngredientAddRequest ingredient1 = new() { Name = "Pomidor" };
        IngredientAddRequest ingredient2 = new() { Name = "Ogórek" };
        IngredientAddRequest ingredient3 = new() { Name = "Rzodkiewka" };

        List<IngredientAddRequest>? ingredientsToAdd = new List<IngredientAddRequest> { ingredient1, ingredient2, ingredient3 };
        List<IngredientResponse>? addedIngredients = new List<IngredientResponse>();

        // Dodanie przepisów do serwisu
        foreach (var ingredient in ingredientsToAdd)
        {
            addedIngredients.Add(_ingredientService.AddIngredient(ingredient));
        }

        // Pobranie wszystkich przepisów
        List<IngredientResponse>? allIngredients = _ingredientService.GetAllIngredients();

        // Sprawdzenie, czy wszystkie dodane przepisy znajdują się w kolekcji
        foreach (IngredientResponse addedIngredient in addedIngredients)
        {
            Assert.Contains(allIngredients, r => r.ID == addedIngredient.ID);
        }
    }

    #endregion
    #region GetFilteredIngredients()
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void GetFilteredIngredients_InvalidSearchString(string? searchString)
    {
        List<IngredientResponse>? allIngredientsList = _ingredientService.GetAllIngredients();

        List<IngredientResponse>? filteredIngredientsList = _ingredientService.GetFilteredIngredients(searchString);

        Assert.Equal(allIngredientsList, filteredIngredientsList);
        Assert.Equal(allIngredientsList!.Count, filteredIngredientsList!.Count);
    }

    [Fact]
    public void GetFilteredIngredients_ValidSearchString_ReturnFilteredIngredients()
    {
        // Arrange
        string? searchString = "Po";
        List<IngredientAddRequest>? ingredientList = new List<IngredientAddRequest>() {
            new() { Name = "Rzodkiewka" },
            new() { Name = "Ogórek" },
            new() { Name = "Pomidor" }
        };
        ingredientList.ForEach(i => _ingredientService.AddIngredient(i));

        List<IngredientResponse>? allIngredientsList = _ingredientService.GetAllIngredients();

        // Act
        List<IngredientResponse>? filteredIngredientsList = _ingredientService.GetFilteredIngredients(searchString);
        List<IngredientResponse>? expectedIngredientsList = allIngredientsList
            .Where(temp => temp.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Assert
        Assert.Equal(expectedIngredientsList, filteredIngredientsList);
        Assert.Equal(expectedIngredientsList!.Count, filteredIngredientsList!.Count);
    }
    #endregion
    #region UpdateIngredient()
    [Fact]
    public void UpdateIngredient_NullIngredient()
    {
        IngredientUpdateRequest? ingredientUpdateRequest = null;
        Assert.Throws<ArgumentNullException>(() =>
        {
            _ingredientService.UpdateIngredient(ingredientUpdateRequest);
        });
    }

    [Fact]
    public void UpdateIngredient_NullID()
    {
        IngredientUpdateRequest ingredientUpdateRequest = new IngredientUpdateRequest
        {
            ID = null,
            Name = "Pomidor",
        };
        Assert.Throws<ArgumentNullException>(() =>
        {
            _ingredientService.UpdateIngredient(ingredientUpdateRequest);
        });
    }

    [Fact]
    public void UpdateIngredient_ValidData_UpdatesSuccessfully()
    {
        IngredientResponse? addedIngredient = _ingredientService.AddIngredient(new() { Name = "Pomidor" });
        IngredientUpdateRequest ingredientToUpdate = new IngredientUpdateRequest
        {
            ID = addedIngredient.ID,
            Name = "test",
        };

        IngredientResponse? updatedIngredient = _ingredientService.UpdateIngredient(ingredientToUpdate);
        IngredientResponse? ingredientFromGetByID = _ingredientService.GetIngredientByID(addedIngredient.ID);

        // Assert
        Assert.NotNull(updatedIngredient);
        Assert.Equal(addedIngredient.ID, updatedIngredient.ID);
        Assert.Equal(ingredientToUpdate.Name, updatedIngredient.Name);

        Assert.NotNull(ingredientFromGetByID);
        Assert.Equal(ingredientToUpdate.Name, ingredientFromGetByID.Name);
    }
    #endregion
    #region DeleteIngredient()
    [Fact]
    public void DeleteIngredient_NullID()
    {
        Guid? invalidIngredientID = null;
        Assert.Throws<ArgumentNullException>(() =>
        {
            _ingredientService.DeleteIngredient(invalidIngredientID);
        });
    }

    [Fact]
    public void DeleteIngredient_InvalidID_ReturnsFalse()
    {
        Guid? invalidIngredientID = Guid.NewGuid();
        bool result = _ingredientService.DeleteIngredient(invalidIngredientID);
        Assert.False(result);
    }

    public void DeleteIngredient_ValidID_DeletesSuccessfully()
    {
        IngredientResponse? addedIngredient = _ingredientService.AddIngredient(new() { Name = "Pomidor" });
        Guid? validIngredientID = addedIngredient.ID;
        bool deleteResult = _ingredientService.DeleteIngredient(validIngredientID);
        IngredientResponse? recipeFromGetByID = _ingredientService.GetIngredientByID(validIngredientID);
        List<IngredientResponse>? allIngredients = _ingredientService.GetAllIngredients();

        Assert.True(deleteResult);
        Assert.Null(recipeFromGetByID);
        Assert.DoesNotContain(addedIngredient, allIngredients);
    }
    #endregion
}
