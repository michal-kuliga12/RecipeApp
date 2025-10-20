using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.DTOs.RecipeIngredientDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApp.Application.Interfaces;

public interface IRecipeIngredientService
{
    RecipeResponse? AddRecipeIngredient(RecipeIngredientAddRequest recipeIngredientAddRequest);
    RecipeResponse? UpdateRecipeIngredient(RecipeIngredientUpdateRequest recipeIngredientUpdateRequest);
    RecipeResponse? DeleteRecipeIngredient(Guid? recipeIngredientID);
}
