using System.ComponentModel.DataAnnotations;
using RecipeApp.Application.DTOs.IngredientDTO;
using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.DTOs.RecipeIngredientDTO;
using RecipeApp.Application.Interfaces;
using RecipeApp.Application.Services;
using RecipeApp.Domain.Entities;
using RecipeApp.Domain.Enums;

namespace RecipeApp.Tests;

public class RecipeServiceTests
{
    private readonly IRecipeService _recipeService;
    private readonly IIngredientService _ingredientService;
    private readonly IRecipeIngredientService _recipeIngredientService;

    public RecipeServiceTests()
    {
        _recipeService = new RecipeService();
        _ingredientService = new IngredientService();
        _recipeIngredientService = new RecipeIngredientService(_recipeService, _ingredientService);
    }

    #region HelperMethods
    private void PopulateRecipeList()
    {
        _recipeService.AddRecipe(new RecipeAddRequest
        {
            Name = "Spaghetti Bolognese",
            Description = "Klasyczne włoskie danie z makaronem spaghetti i sosem mięsnym.",
            Author = "Jan Kowalski",
            Category = Category.MainCourse,
            PreparationTime = 45,
            RecipeIngredients = new List<RecipeIngredient> { new() { ID = Guid.NewGuid(), RecipeID = Guid.NewGuid(), IngredientID = Guid.NewGuid(), Quantity = 400, Unit = Unit.Gram } },
            Servings = 4,
            Rating = 4.5,
            ImageUrl = "https://example.com/images/spaghetti.jpg",
            CreatedAt = DateTime.Now
        });
        _recipeService.AddRecipe(new RecipeAddRequest
        {
            Name = "Sałatka Cezar",
            Description = "Lekka i szybka w przygotowaniu sałatka z kurczakiem i parmezanem.",
            Author = "Anna Nowak",
            Category = Category.Salads,
            PreparationTime = 20,
            RecipeIngredients = new List<RecipeIngredient> { new() { ID = Guid.NewGuid(), RecipeID = Guid.NewGuid(), IngredientID = Guid.NewGuid(), Quantity = 400, Unit = Unit.Gram } },
            Servings = 2,
            Rating = 4.0,
            ImageUrl = "https://example.com/images/cezar.jpg",
            CreatedAt = DateTime.Now
        });
        _recipeService.AddRecipe(new RecipeAddRequest
        {
            Name = "Szarlotka",
            Description = "Tradycyjne polskie ciasto z jabłkami, idealne na deser.",
            Author = "Piotr Wiśniewski",
            Category = Category.PastriesAndDesserts,
            PreparationTime = 90,
            RecipeIngredients = new List<RecipeIngredient> { new() { ID = Guid.NewGuid(), RecipeID = Guid.NewGuid(), IngredientID = Guid.NewGuid(), Quantity = 400, Unit = Unit.Gram } },
            Servings = 8,
            Rating = 5.0,
            ImageUrl = "https://example.com/images/szarlotka.jpg",
            CreatedAt = DateTime.Now
        });
    }
    private RecipeResponse? PopulateOneRecipe_ReturnsRecipeResponse()
    {
        return _recipeService.AddRecipe(new RecipeAddRequest
        {
            Name = "Spaghetti Bolognese",
            Description = "Klasyczne włoskie danie z makaronem spaghetti i sosem mięsnym.",
            Author = "Jan Kowalski",
            Category = Category.MainCourse,
            PreparationTime = 45,
            RecipeIngredients = new List<RecipeIngredient> { },
            Servings = 4,
            Rating = 4.5,
            ImageUrl = "https://example.com/images/spaghetti.jpg",
            CreatedAt = DateTime.Now
        });
    }

    private IngredientResponse? PopulateOneIngredient_ReturnsIngredientResponse()
    {
        return _ingredientService.AddIngredient(new IngredientAddRequest()
        {
            Name = "Pomidor"
        });
    }
    #endregion

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
            Category = Category.MainCourse,
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
            Category = Category.MainCourse,
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
        RecipeResponse recipeResponseFromAdd = PopulateOneRecipe_ReturnsRecipeResponse();
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
            Category = Category.MainCourse,
            CreatedAt = DateTime.UtcNow,
            RecipeIngredients = new List<RecipeIngredient> { new() { ID = Guid.NewGuid(), RecipeID = Guid.NewGuid(), IngredientID = Guid.NewGuid(), Quantity = 400, Unit = Unit.Gram } }
        };
        RecipeAddRequest recipeRequestToAdd2 = new RecipeAddRequest
        {
            Name = "Spaghetti Bolognese",
            Author = "Jan Kowalski",
            Category = Category.MainCourse,
            CreatedAt = DateTime.UtcNow,
            RecipeIngredients = new List<RecipeIngredient> { new() { ID = Guid.NewGuid(), RecipeID = Guid.NewGuid(), IngredientID = Guid.NewGuid(), Quantity = 400, Unit = Unit.Gram } }
        };
        RecipeAddRequest recipeRequestToAdd3 = new RecipeAddRequest
        {
            Name = "Spaghetti Bolognese",
            Author = "Jan Kowalski",
            Category = Category.MainCourse,
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
    #region GetFilteredRecipes()
    [Fact]
    public void GetFilteredRecipes_ProperSearchValues()
    {
        PopulateRecipeList();
        List<RecipeResponse>? allRecipes = _recipeService.GetAllRecipes();
        List<RecipeResponse> expectedFilteredRecipes = new List<RecipeResponse>();

        string? searchString = "Spaghetti";
        string? searchBy = nameof(RecipeResponse.Name);
        List<RecipeResponse> filteredRecipes = new List<RecipeResponse>();

        filteredRecipes = _recipeService.GetFilteredRecipes(searchBy, searchString);
        expectedFilteredRecipes = allRecipes.Where(
            r => r.Name != null
            && r.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
        ).ToList();

        Assert.Equal(expectedFilteredRecipes.Count, filteredRecipes.Count);
        Assert.All(filteredRecipes, r => Assert.Contains(searchString, r.Name, StringComparison.OrdinalIgnoreCase));
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void GetFilteredRecipes_NullOrEmptySearchName(string searchString)
    {
        PopulateRecipeList();
        List<RecipeResponse> allRecipes = _recipeService.GetAllRecipes();

        string? searchBy = nameof(RecipeResponse.Name);
        List<RecipeResponse> filteredRecipes = _recipeService.GetFilteredRecipes(searchBy, searchString);

        Assert.Equal(allRecipes, filteredRecipes);
    }

    [Fact]
    public void GetFilteredRecipes_NullSearchBy()
    {
        List<RecipeResponse> allRecipes = _recipeService.GetAllRecipes();
        string? searchString = "sałatka";
        string? searchBy = null;

        List<RecipeResponse> filteredRecipes = _recipeService.GetFilteredRecipes(searchBy, searchString);

        Assert.Equal(allRecipes.Count, filteredRecipes.Count);
        //Assert.Throws<ArgumentNullException>(() =>
        //{
        //    var filteredRecipes = _recipeService.GetFilteredRecipes(searchBy, searchString);
        //});
    }

    [Fact]
    public void GetFilteredRecipes_NoResults()
    {
        PopulateRecipeList();
        List<RecipeResponse> allRecipes = _recipeService.GetAllRecipes();

        string? searchString = "Paella z kurczakiem";
        string? searchBy = nameof(RecipeResponse.Name);
        List<RecipeResponse> filteredRecipes = _recipeService.GetFilteredRecipes(searchBy, searchString);

        Assert.Empty(filteredRecipes);
    }

    #endregion
    #region GetSortedRecipes()
    [Fact]
    public void GetSortedRecipes_ReturnAscendingOrder()
    {
        PopulateRecipeList();
        List<RecipeResponse> allRecipes = _recipeService.GetAllRecipes();

        List<RecipeResponse>? sortedRecipes = _recipeService.GetSortedRecipes(allRecipes, nameof(RecipeResponse.Author), true);

        Assert.Equal("Anna Nowak", sortedRecipes[0].Author);
        Assert.Equal("Jan Kowalski", sortedRecipes[1].Author);
        Assert.Equal("Piotr Wiśniewski", sortedRecipes[2].Author);
    }

    [Fact]
    public void GetSortedRecipes_ReturnDescendingOrder()
    {
        PopulateRecipeList();
        List<RecipeResponse> allRecipes = _recipeService.GetAllRecipes();

        List<RecipeResponse>? sortedRecipes = _recipeService.GetSortedRecipes(allRecipes, nameof(RecipeResponse.Author), false);

        Assert.Equal("Anna Nowak", sortedRecipes[2].Author);
        Assert.Equal("Jan Kowalski", sortedRecipes[1].Author);
        Assert.Equal("Piotr Wiśniewski", sortedRecipes[0].Author);
    }

    [Fact]
    public void GetSortedRecipes_CheckCount()
    {
        PopulateRecipeList();
        List<RecipeResponse> allRecipes = _recipeService.GetAllRecipes();

        List<RecipeResponse>? sortedRecipes = _recipeService.GetSortedRecipes(allRecipes, nameof(RecipeResponse.Author), false);

        Assert.Equal(sortedRecipes.Count, allRecipes.Count);
    }

    [Fact]
    public void GetSortedRecipes_EmptyList_ReturnEmpty()
    {
        List<RecipeResponse>? emptyRecipesList = _recipeService.GetSortedRecipes(new List<RecipeResponse>(), nameof(RecipeResponse.Author), false);
        Assert.Empty(emptyRecipesList);
    }

    [Fact]
    public void GetSortedRecipes_SingleElement_ReturnsSameList()
    {
        List<RecipeResponse> single = new List<RecipeResponse> { new RecipeResponse { Author = "Test" } };

        List<RecipeResponse>? result = _recipeService.GetSortedRecipes(single, nameof(RecipeResponse.Author), true);

        Assert.Single(result);
        Assert.Equal("Test", result[0].Author);
    }

    [Fact]
    public void GetSortedRecipes_InvalidProperty_ReturnUnsorted()
    {
        PopulateRecipeList();
        List<RecipeResponse> allRecipes = _recipeService.GetAllRecipes();

        List<RecipeResponse> sortedRecipes = _recipeService.GetSortedRecipes(allRecipes, "abc", false);

        Assert.Equal(allRecipes.Count, sortedRecipes.Count);
    }

    [Fact]
    public void GetSortedRecipes_NullList_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _recipeService.GetSortedRecipes(null, nameof(RecipeResponse.Author), true));
    }
    #endregion
    #region UpdateRecipe()
    [Fact]
    public void UpdateRecipe_NullRecipe()
    {
        RecipeUpdateRequest? recipeUpdateRequest = null;
        Assert.Throws<ArgumentNullException>(() =>
        {
            _recipeService.UpdateRecipe(recipeUpdateRequest);
        });
    }

    [Fact]
    public void UpdateRecipe_NullID()
    {
        RecipeUpdateRequest recipeUpdateRequest = new RecipeUpdateRequest
        {
            ID = null,
            Name = "Spaghetti Bolognese",
            Description = "Klasyczne włoskie danie z makaronem spaghetti i sosem mięsnym.",
            Author = "Jan Kowalski",
            Category = Category.MainCourse,
            PreparationTime = 45,
            RecipeIngredients = new List<RecipeIngredient> { new() { ID = Guid.NewGuid(), RecipeID = Guid.NewGuid(), IngredientID = Guid.NewGuid(), Quantity = 400, Unit = Unit.Gram } },
            Servings = 4,
            Rating = 4.5,
            ImageUrl = "https://example.com/images/spaghetti.jpg",
            CreatedAt = DateTime.Now
        };
        Assert.Throws<ArgumentNullException>(() =>
        {
            _recipeService.UpdateRecipe(recipeUpdateRequest);
        });
    }

    [Fact]
    public void UpdateRecipe_ValidData_UpdatesSuccessfully()
    {
        RecipeResponse addedRecipe = PopulateOneRecipe_ReturnsRecipeResponse();
        RecipeUpdateRequest recipeToUpdate = new RecipeUpdateRequest
        {
            ID = addedRecipe.ID,
            Name = "Spaghetti Carbonara",
            Description = "Włoskie danie z makaronem spaghetti, jajkami, serem pecorino i boczkiem.",
            Author = "Jan Kowalski",
            Category = Category.MainCourse,
            PreparationTime = 30,
            RecipeIngredients = new List<RecipeIngredient> { new() { ID = Guid.NewGuid(), RecipeID = Guid.NewGuid(), IngredientID = Guid.NewGuid(), Quantity = 400, Unit = Unit.Gram } },
            Servings = 4,
            Rating = 4.7,
            ImageUrl = "https://example.com/images/spaghetti-carbonara.jpg",
            CreatedAt = addedRecipe.CreatedAt // zachowujemy oryginalną datę utworzenia
        };

        RecipeResponse updatedRecipe = _recipeService.UpdateRecipe(recipeToUpdate);
        RecipeResponse? recipeFromGetByID = _recipeService.GetRecipeByID(addedRecipe.ID);

        // Assert
        Assert.NotNull(updatedRecipe);
        Assert.Equal(addedRecipe.ID, updatedRecipe.ID);
        Assert.Equal(recipeToUpdate.Name, updatedRecipe.Name);
        Assert.Equal(recipeToUpdate.Description, updatedRecipe.Description);
        Assert.Equal(recipeToUpdate.Author, updatedRecipe.Author);
        Assert.Equal(recipeToUpdate.Category, updatedRecipe.Category);
        Assert.Equal(recipeToUpdate.PreparationTime, updatedRecipe.PreparationTime);
        Assert.Equal(recipeToUpdate.Servings, updatedRecipe.Servings);
        Assert.Equal(recipeToUpdate.Rating, updatedRecipe.Rating);
        Assert.Equal(recipeToUpdate.ImageUrl, updatedRecipe.ImageUrl);
        Assert.Equal(addedRecipe.CreatedAt, updatedRecipe.CreatedAt);

        Assert.NotNull(recipeFromGetByID);
        Assert.Equal(recipeToUpdate.Name, recipeFromGetByID.Name);
        Assert.Equal(recipeToUpdate.Rating, recipeFromGetByID.Rating);
    }

    #endregion
    #region DeleteRecipe()
    [Fact]
    public void DeleteRecipe_NullID()
    {
        Guid? invalidRecipeID = null;
        Assert.Throws<ArgumentNullException>(() =>
        {
            _recipeService.DeleteRecipe(invalidRecipeID);
        });
    }

    [Fact]
    public void DeleteRecipe_InvalidID_ReturnsFalse()
    {
        Guid? invalidRecipeID = Guid.NewGuid();
        bool result = _recipeService.DeleteRecipe(invalidRecipeID);
        Assert.False(result);
    }

    public void DeleteRecipe_ValidID_DeletesSuccessfully()
    {
        RecipeResponse addedRecipe = PopulateOneRecipe_ReturnsRecipeResponse();
        Guid? validRecipeID = addedRecipe.ID;
        bool deleteResult = _recipeService.DeleteRecipe(validRecipeID);
        RecipeResponse? recipeFromGetByID = _recipeService.GetRecipeByID(validRecipeID);
        List<RecipeResponse> allRecipes = _recipeService.GetAllRecipes();
        Assert.True(deleteResult);
        Assert.Null(recipeFromGetByID);
        Assert.DoesNotContain(addedRecipe, allRecipes);
    }
    #endregion

    #region AddRecipeIngredient
    [Fact]
    public void AddRecipeIngredient_ValidRequest()
    {
        RecipeResponse? recipe = PopulateOneRecipe_ReturnsRecipeResponse();
        Guid recipeID = recipe.ID;

        IngredientResponse ingredient = PopulateOneIngredient_ReturnsIngredientResponse();
        Guid ingredientID = ingredient.ID;

        RecipeIngredientAddRequest recipeAddIngredientRequest = new()
        {
            IngredientID = ingredientID,
            RecipeID = recipeID,
            Quantity = 1,
            Unit = Unit.Piece
        };

        RecipeResponse? recipeWithAddedIngredient = _recipeIngredientService.AddRecipeIngredient(recipeAddIngredientRequest);

        Assert.Single(recipeWithAddedIngredient.RecipeIngredients);
        var addedIngredient = recipeWithAddedIngredient.RecipeIngredients.First();

        Assert.Equal(recipeAddIngredientRequest.IngredientID, addedIngredient.IngredientID);
        Assert.Equal(recipeAddIngredientRequest.RecipeID, addedIngredient.RecipeID);
        Assert.Equal(recipeAddIngredientRequest.Quantity, addedIngredient.Quantity);
        Assert.Equal(recipeAddIngredientRequest.Unit, addedIngredient.Unit);

    }

    [Fact]
    public void AddRecipeIngredient_InvalidQuantity_ShouldFailValidation()
    {
        var request = new RecipeIngredientAddRequest
        {
            IngredientID = Guid.NewGuid(),
            RecipeID = Guid.NewGuid(),
            Quantity = 0,  // niepoprawna ilość
            Unit = Unit.Gram
        };

        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(request);
        bool isValid = Validator.TryValidateObject(request, context, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, r => r.ErrorMessage.Contains("Ilość musi być większa niż 0 i mniejsza niż 10 000"));
    }

    [Fact]
    public void AddRecipeIngredient_InvalidIngredientID()
    {
        // Arrange
        var request = new RecipeIngredientAddRequest
        {
            IngredientID = Guid.Empty,
            RecipeID = Guid.NewGuid(),  // odpowiednik "braku wartości"
            Quantity = 10,
            Unit = Unit.Gram
        };

        // Act
        var response = _recipeIngredientService.AddRecipeIngredient(request);

        Assert.Null(response);
    }
    [Fact]
    public void AddRecipeIngredient_EmptyRecipeID_ReturnsError()
    {
        // Arrange
        var request = new RecipeIngredientAddRequest
        {
            IngredientID = Guid.NewGuid(),
            RecipeID = Guid.Empty,  // odpowiednik "braku wartości"
            Quantity = 10,
            Unit = Unit.Gram
        };

        // Act
        var response = _recipeIngredientService.AddRecipeIngredient(request);

        Assert.Null(response);
    }

    [Fact]
    public void AddRecipeIngredient_MissingUnit_ShouldFailValidation()
    {
        var request = new RecipeIngredientAddRequest
        {
            IngredientID = Guid.NewGuid(),
            RecipeID = Guid.NewGuid(),
            Quantity = 10,
            Unit = null
        };

        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(request);
        bool isValid = Validator.TryValidateObject(request, context, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, r => r.ErrorMessage.Contains("Jednostka jest wymagana"));
    }

    [Fact]
    public void AddRecipeIngredient_AlreadyExists_ReturnsError()
    {
        RecipeResponse? recipe = PopulateOneRecipe_ReturnsRecipeResponse();
        Guid recipeID = recipe.ID;

        IngredientResponse ingredient = PopulateOneIngredient_ReturnsIngredientResponse();
        Guid ingredientID = ingredient.ID;

        RecipeIngredientAddRequest recipeAddIngredientRequest = new()
        {
            IngredientID = ingredientID,
            RecipeID = recipeID,
            Quantity = 1,
            Unit = Unit.Piece
        };

        RecipeResponse? recipeWithAddedIngredient = _recipeIngredientService.AddRecipeIngredient(recipeAddIngredientRequest);

        Assert.Throws<InvalidOperationException>(() =>
        {
            _recipeIngredientService.AddRecipeIngredient(recipeAddIngredientRequest);
        });
    }

    #endregion

    #region UpdateRecipeIngredient
    [Fact]
    public void UpdateRecipeIngredient_ValidRequest()
    {
        // Arrange
        IngredientResponse? ingredient = PopulateOneIngredient_ReturnsIngredientResponse();
        Guid ingredientID = ingredient.ID;

        RecipeResponse? recipe = _recipeService.AddRecipe(new RecipeAddRequest
        {
            Name = "Spaghetti Bolognese",
            Description = "Klasyczne włoskie danie z makaronem spaghetti i sosem mięsnym.",
            Author = "Jan Kowalski",
            Category = Category.MainCourse,
            PreparationTime = 45,
            RecipeIngredients = new List<RecipeIngredient>(),
            Servings = 4,
            Rating = 4.5,
            ImageUrl = "https://example.com/images/spaghetti.jpg",
            CreatedAt = DateTime.Now
        });
        RecipeResponse? recipeWithIngredient = _recipeIngredientService.AddRecipeIngredient(new RecipeIngredientAddRequest
        {
            IngredientID = ingredientID,
            RecipeID = recipe.ID,
            Quantity = 1,
            Unit = Unit.Piece
        });

        //Act
        RecipeIngredient? addedIngredient = recipeWithIngredient.RecipeIngredients.First();
        RecipeIngredientUpdateRequest recipeUpdateIngredientRequest = new()
        {
            ID = addedIngredient.ID,
            IngredientID = ingredientID,
            RecipeID = recipeWithIngredient.ID,
            Quantity = 3,
            Unit = Unit.Gram
        };
        RecipeResponse? recipeWithUpdatedIngredient = _recipeIngredientService.UpdateRecipeIngredient(recipeUpdateIngredientRequest);
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
    public void UpdateRecipeIngredient_ShouldFail_WhenIngredientNotFound()
    {
        // Arrange
        Guid nonExistentId = Guid.NewGuid();
        RecipeIngredientUpdateRequest request = new RecipeIngredientUpdateRequest
        {
            ID = nonExistentId,
            IngredientID = Guid.NewGuid(),
            RecipeID = Guid.NewGuid(),
            Quantity = 5,
            Unit = Unit.Gram
        };

        // Act
        RecipeResponse? result = _recipeIngredientService.UpdateRecipeIngredient(request);

        // Assert
        Assert.Null(result); // lub inny mechanizm obsługi błędu, jeśli rzucany jest wyjątek
    }

    [Fact]
    public void UpdateRecipeIngredient_ShouldFail_WhenIdIsEmpty()
    {
        // Arrange
        RecipeIngredientUpdateRequest request = new RecipeIngredientUpdateRequest
        {
            ID = Guid.Empty,
            IngredientID = Guid.NewGuid(),
            RecipeID = Guid.NewGuid(),
            Quantity = 5,
            Unit = Unit.Gram
        };

        // Act
        RecipeResponse? result = _recipeIngredientService.UpdateRecipeIngredient(request);

        // Assert
        Assert.Null(result); // lub Assert.Throws<ValidationException>(() => ...)
    }

    // Update recipe
    #endregion

}