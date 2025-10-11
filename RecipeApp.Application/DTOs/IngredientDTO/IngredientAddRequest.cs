using System.ComponentModel.DataAnnotations;
using RecipeApp.Domain.Entities;

namespace RecipeApp.Application.DTOs.IngredientDTO;

public class IngredientAddRequest
{
    [Required(ErrorMessage = "Nazwa produktu jest wymagana")]
    [StringLength(50, MinimumLength = 2)]
    public string? Name { get; set; }

    public Ingredient ToIngredient()
        => new Ingredient() { Name = Name };
}
