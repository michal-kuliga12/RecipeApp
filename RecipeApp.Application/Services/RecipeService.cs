
using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.DTOs.RecipeIngredientDTO;
using RecipeApp.Application.Helpers;
using RecipeApp.Application.Interfaces;
using RecipeApp.Domain.Entities;
using RecipeApp.Domain.Enums;

namespace RecipeApp.Application.Services;

public class RecipeService : IRecipeService
{
    private readonly List<Recipe> _recipes;

    public RecipeService(bool initialize = true)
    {
        _recipes = new List<Recipe>();
        
        if (initialize && !_recipes.Any())
        {
            AddRecipe(new RecipeAddRequest { Name = "Burger Classic", Description = "Soczysty burger z serem, sałatą i pomidorem", Author = "Anna Kowalska", Category = Category.MainCourse, PreparationTime = 20, RecipeIngredients = new List<Domain.Entities.RecipeIngredient> { }, Servings = 2, Rating = 4.5, ImageUrl = "https://foodish-api.com/images/burger/burger19.jpg", CreatedAt = DateTime.Now.AddDays(-10) });
            AddRecipe(new RecipeAddRequest { Name = "Pizza Margherita", Description = "Klasyczna pizza z sosem pomidorowym i mozzarellą", Author = "Jan Nowak", Category = Category.MainCourse, PreparationTime = 25, RecipeIngredients = new List<Domain.Entities.RecipeIngredient> { }, Servings = 4, Rating = 4.8, ImageUrl = "https://foodish-api.com/images/pizza/pizza66.jpg", CreatedAt = DateTime.Now.AddDays(-5) });
            AddRecipe(new RecipeAddRequest { Name = "Spaghetti Bolognese", Description = "Makaron z sosem mięsnym i pomidorami", Author = "Maria Zielińska", Category = Category.MainCourse, PreparationTime = 35, RecipeIngredients = new List<Domain.Entities.RecipeIngredient> { }, Servings = 3, Rating = 4.2, ImageUrl = "https://foodish-api.com/images/pasta/pasta14.jpg", CreatedAt = DateTime.Now.AddDays(-20) });
            AddRecipe(new RecipeAddRequest { Name = "Tort Czekoladowy", Description = "Wilgotny tort czekoladowy z kremem", Author = "Katarzyna Wiśniewska", Category = Category.PastriesAndDesserts, PreparationTime = 60, RecipeIngredients = new List<Domain.Entities.RecipeIngredient> { }, Servings = 8, Rating = 4.9, ImageUrl = "https://foodish-api.com/images/dessert/dessert12.jpg", CreatedAt = DateTime.Now.AddDays(-2) });
            AddRecipe(new RecipeAddRequest { Name = "Sałatka Grecka", Description = "Sałatka z pomidorów, ogórka, sera feta i oliwek", Author = "Piotr Nowicki", Category = Category.Salads, PreparationTime = 15, RecipeIngredients = new List<Domain.Entities.RecipeIngredient> { }, Servings = 2, Rating = 4.0, ImageUrl = "https://foodish-api.com/images/pasta/pasta24.jpg", CreatedAt = DateTime.Now.AddDays(-7) });
            AddRecipe(new RecipeAddRequest { Name = "Samosa Pikantna", Description = "Pikantna przekąska z ziemniakami i groszkiem", Author = "Agnieszka Bąk", Category = Category.Snacks, PreparationTime = 30, RecipeIngredients = new List<Domain.Entities.RecipeIngredient> { }, Servings = 6, Rating = 4.3, ImageUrl = "https://foodish-api.com/images/samosa/samosa16.jpg", CreatedAt = DateTime.Now.AddDays(-12) });
            AddRecipe(new RecipeAddRequest { Name = "Curry Z Kurczakiem", Description = "Aromatyczne curry z kurczakiem i warzywami", Author = "Łukasz Szymański", Category = Category.MainCourse, PreparationTime = 40, RecipeIngredients = new List<Domain.Entities.RecipeIngredient> { }, Servings = 4, Rating = 4.6, ImageUrl = "https://foodish-api.com/images/biryani/biryani32.jpg", CreatedAt = DateTime.Now.AddDays(-18) });
            AddRecipe(new RecipeAddRequest { Name = "Dosa", Description = "Cienkie placki ryżowo-soczewicowe z farszem", Author = "Sunita Patel", Category = Category.MainCourse, PreparationTime = 20, RecipeIngredients = new List<Domain.Entities.RecipeIngredient> { }, Servings = 3, Rating = 4.4, ImageUrl = "https://foodish-api.com/images/dosa/dosa31.jpg", CreatedAt = DateTime.Now.AddDays(-8) });
            AddRecipe(new RecipeAddRequest { Name = "Lody Waniliowe", Description = "Kremowe lody waniliowe z bitą śmietaną", Author = "Elżbieta Kowal", Category = Category.PastriesAndDesserts, PreparationTime = 10, RecipeIngredients = new List<Domain.Entities.RecipeIngredient> { }, Servings = 4, Rating = 4.7, ImageUrl = "https://foodish-api.com/images/dessert/dessert25.jpg", CreatedAt = DateTime.Now.AddDays(-1) });
            AddRecipe(new RecipeAddRequest { Name = "Butter Chicken", Description = "Indyjski kurczak w maślanym sosie pomidorowym", Author = "Ravi Singh", Category = Category.MainCourse, PreparationTime = 50, RecipeIngredients = new List<Domain.Entities.RecipeIngredient> { }, Servings = 4, Rating = 4.8, ImageUrl = "https://foodish-api.com/images/butter-chicken/butter-chicken8.jpg", CreatedAt = DateTime.Now.AddDays(-15) });
        }
    }

    public RecipeResponse AddRecipe(RecipeAddRequest? recipeAddRequest)
    {
        if (recipeAddRequest is null)
            throw new ArgumentNullException("RecipeAddRequest nie może być null");

        bool isModelValid = ValidationHelper.ValidateModel(recipeAddRequest);

        if (!isModelValid)
            throw new ArgumentNullException("Model jest niepoprawny");

        Recipe recipe = recipeAddRequest.ToRecipe();
        recipe.ID = Guid.NewGuid();
        _recipes.Add(recipe);
        RecipeResponse recipeResponse = recipe.ToRecipeResponse();

        return recipeResponse;

    }
    public RecipeResponse? GetRecipeByID(Guid? recipeID)
    {
        if (recipeID == null)
            return null;

        Recipe? recipeFound = _recipes.SingleOrDefault(temp => temp.ID == recipeID);

        if (recipeFound == null)
            return null;

        return recipeFound.ToRecipeResponse();
    }
    public List<RecipeResponse>? GetAllRecipes()
    => _recipes.Select(temp => temp.ToRecipeResponse()).ToList();
    public List<RecipeResponse>? GetFilteredRecipes(string searchBy, string? searchString)
    {
        List<RecipeResponse> allRecipes = GetAllRecipes();
        List<RecipeResponse> filteredRecipes = allRecipes;

        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            return filteredRecipes;

        switch (searchBy)
        {
            case nameof(RecipeResponse.Name):
                filteredRecipes = allRecipes.Where(temp => !String.IsNullOrEmpty(temp.Name)
                    && temp.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
                break;
            case nameof(RecipeResponse.Description):
                filteredRecipes = allRecipes.Where(temp => !String.IsNullOrEmpty(temp.Description)
                    && temp.Description.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
                break;
            case nameof(RecipeResponse.Author):
                filteredRecipes = allRecipes.Where(temp => !String.IsNullOrEmpty(temp.Author)
                    && temp.Author.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
                break;
            case nameof(RecipeResponse.Category):
                filteredRecipes = allRecipes.Where(temp => temp.Category != null
                    && EnumDisplayHelper.GetDisplayName(temp.Category).Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
                break;
            case nameof(RecipeResponse.PreparationTime):
                filteredRecipes = allRecipes.Where(temp => temp.PreparationTime != null && temp.PreparationTime >= 1
                    && temp.PreparationTime.ToString().Equals(searchString)).ToList();
                break;
            case nameof(RecipeResponse.RecipeIngredients):
                filteredRecipes = allRecipes.Where(temp => temp.RecipeIngredients != null
                    && temp.RecipeIngredients.Any(ing => ing.Ingredient.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))).ToList();
                break;
            case nameof(RecipeResponse.Servings):
                filteredRecipes = allRecipes.Where(temp => temp.Servings != null && temp.Servings >= 1
                    && temp.Servings.ToString().Equals(searchString)).ToList();
                break;
            case nameof(RecipeResponse.Rating):
                filteredRecipes = allRecipes.Where(temp => temp.Rating != null
                    && temp.Rating.ToString().Equals(searchString)).ToList();
                break;
            default:
                filteredRecipes = allRecipes;
                break;
        }
        return filteredRecipes;
    }
    public List<RecipeResponse>? GetSortedRecipes(List<RecipeResponse> recipeList, string sortBy, bool ascending = true)
    {
        if (recipeList == null)
            throw new ArgumentNullException("Lista przepisów do sortowania nie może być null");

        if (String.IsNullOrEmpty(sortBy))
            return recipeList;

        List<RecipeResponse> sortedRecipes = (sortBy, ascending) switch
        {
            (nameof(RecipeResponse.Name), ascending: true) => recipeList.OrderBy(temp => temp.Name, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Name), ascending: false) => recipeList.OrderByDescending(temp => temp.Name, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Description), ascending: true) => recipeList.OrderBy(temp => temp.Description, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Description), ascending: false) => recipeList.OrderByDescending(temp => temp.Description, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Author), ascending: true) => recipeList.OrderBy(temp => temp.Author, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Author), ascending: false) => recipeList.OrderByDescending(temp => temp.Author, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(RecipeResponse.Category), ascending: true) => recipeList.OrderBy(temp => temp.Category).ToList(),
            (nameof(RecipeResponse.Category), ascending: false) => recipeList.OrderByDescending(temp => temp.Category).ToList(),
            (nameof(RecipeResponse.PreparationTime), ascending: true) => recipeList.OrderBy(temp => temp.PreparationTime).ToList(),
            (nameof(RecipeResponse.PreparationTime), ascending: false) => recipeList.OrderByDescending(temp => temp.PreparationTime).ToList(),
            (nameof(RecipeResponse.Servings), ascending: true) => recipeList.OrderBy(temp => temp.Servings).ToList(),
            (nameof(RecipeResponse.Servings), ascending: false) => recipeList.OrderByDescending(temp => temp.Servings).ToList(),
            (nameof(RecipeResponse.Rating), ascending: true) => recipeList.OrderBy(temp => temp.Rating).ToList(),
            (nameof(RecipeResponse.Rating), ascending: false) => recipeList.OrderByDescending(temp => temp.Rating).ToList(),
            (nameof(RecipeResponse.CreatedAt), ascending: true) => recipeList.OrderBy(temp => temp.CreatedAt).ToList(),
            (nameof(RecipeResponse.CreatedAt), ascending: false) => recipeList.OrderByDescending(temp => temp.CreatedAt).ToList(),
            _ => recipeList
        };

        return sortedRecipes;
    }
    public RecipeResponse UpdateRecipe(RecipeUpdateRequest recipeUpdateRequest)
    {
        if (recipeUpdateRequest is null)
            throw new ArgumentNullException("RecipeUpdateRequest nie może być null");

        bool isModelValid = ValidationHelper.ValidateModel(recipeUpdateRequest);

        if (!isModelValid)
            throw new ArgumentNullException("Model jest niepoprawny");

        Recipe? recipeFound = _recipes.SingleOrDefault(temp => temp.ID == recipeUpdateRequest.ID);

        if (recipeFound is null)
            return null;

        recipeFound.Name = recipeUpdateRequest.Name;
        recipeFound.Description = recipeUpdateRequest.Description;
        recipeFound.Author = recipeUpdateRequest.Author;
        recipeFound.Category = recipeUpdateRequest.Category;
        recipeFound.PreparationTime = recipeUpdateRequest.PreparationTime;
        recipeFound.RecipeIngredients = recipeUpdateRequest.RecipeIngredients;
        recipeFound.Servings = recipeUpdateRequest.Servings;
        recipeFound.Rating = recipeUpdateRequest.Rating;
        recipeFound.ImageUrl = recipeUpdateRequest.ImageUrl;

        return recipeFound.ToRecipeResponse();
    }
    public bool DeleteRecipe(Guid? recipeID)
    {
        if (recipeID is null)
            throw new ArgumentNullException("RecipeID nie może być null");

        Recipe? recipeToDelete = _recipes.SingleOrDefault(temp => temp.ID == recipeID);

        if (recipeToDelete == null)
            return false;

        _recipes.Remove(recipeToDelete);
        return true;
    }
    public Recipe? GetRecipeEntityByID(Guid? recipeID)
    {
        if (recipeID == null)
            return null;
        Recipe? recipeFound = _recipes.SingleOrDefault(temp => temp.ID == recipeID);
        return recipeFound;
    }
}
