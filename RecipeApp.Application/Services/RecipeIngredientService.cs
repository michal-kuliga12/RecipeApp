using RecipeApp.Application.DTOs.IngredientDTO;
using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.DTOs.RecipeIngredientDTO;
using RecipeApp.Application.Helpers;
using RecipeApp.Application.Interfaces;
using RecipeApp.Domain.Entities;

namespace RecipeApp.Application.Services;

public class RecipeIngredientService : IRecipeIngredientService
{
    private readonly IRecipeService _recipeService;
    private readonly IIngredientService _ingredientService;

    public RecipeIngredientService(IRecipeService recipeService, IIngredientService ingredientService)
    {
        _recipeService = recipeService;
        _ingredientService = ingredientService;
    }

    public RecipeResponse? AddRecipeIngredient(RecipeIngredientAddRequest recipeIngredientAddRequest)
    {
        if (recipeIngredientAddRequest is null)
            throw new ArgumentNullException("RecipeIngredientAddRequest nie może być null");

        Guid? recipeID = recipeIngredientAddRequest?.RecipeID;
        if (recipeID is null)
            throw new ArgumentNullException("RecipeID nie może być null");

        Recipe? recipeFound = _recipeService.GetRecipeEntityByID(recipeID);

        if (recipeFound is null)
            return null;

        bool isModelValid = ValidationHelper.ValidateModel(recipeIngredientAddRequest);
        if (!isModelValid)
            throw new ArgumentException("Niepoprawne wartości w RecipeIngredientAddRequest");

        bool hasIngredientId = recipeIngredientAddRequest.IngredientID.HasValue;
        bool hasIngredientName = !string.IsNullOrWhiteSpace(recipeIngredientAddRequest.IngredientName);

        if (!hasIngredientId && !hasIngredientName)
            throw new ArgumentNullException(nameof(recipeIngredientAddRequest),
                "Musisz podać IngredientID lub IngredientName.");

        if (!hasIngredientId)
        {
            string ingredientName = recipeIngredientAddRequest.IngredientName!;
            IngredientResponse? filteredIngredient = _ingredientService
                .GetFilteredIngredients(ingredientName)
                .FirstOrDefault(i => i.Name.Equals(ingredientName, StringComparison.InvariantCultureIgnoreCase));
            if (filteredIngredient is not null)
            {
                recipeIngredientAddRequest.IngredientID = filteredIngredient?.ID;
                
            } else
            {
                IngredientAddRequest ingredientToAdd = new()
                {
                    Name = ingredientName,
                };
                IngredientResponse? ingredientFromAdd = _ingredientService.AddIngredient(ingredientToAdd);
                recipeIngredientAddRequest.IngredientID = ingredientFromAdd.ID;
            }   
        }

        // jeśli podano nazwę (np. do walidacji czy nie ma konfliktu)
        if (!hasIngredientName)
        {
            // logika związana z nazwą
        }


        RecipeIngredient recipeIngredient = recipeIngredientAddRequest.ToRecipeIngredient();
        recipeIngredient.ID = Guid.NewGuid();

        recipeFound.RecipeIngredients.Add(recipeIngredient);
        return recipeFound.ToRecipeResponse();
    }

    public RecipeResponse? UpdateRecipeIngredient(RecipeIngredientUpdateRequest recipeIngredientUpdateRequest)
    {
        throw new NotImplementedException();
    }

    public RecipeResponse? DeleteRecipeIngredient(Guid? recipeIngredientID)
    {
        throw new NotImplementedException();
    }
}
