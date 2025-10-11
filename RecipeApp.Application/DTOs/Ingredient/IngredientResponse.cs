using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Application.DTOs.Ingredient;

public class IngredientResponse
{

    [Required]
    public Guid ID { get; set; }
    [Required(ErrorMessage = "Nazwa produktu jest wymagana")]
    [Range(2, 50)]
    public string? Name { get; set; }
}
