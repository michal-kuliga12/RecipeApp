using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Application.DTOs.IngredientDTO;

public class IngredientUpdateRequest
{
    [Required]
    public Guid? ID { get; set; }
    [Required(ErrorMessage = "Nazwa produktu jest wymagana")]
    [StringLength(50, MinimumLength = 2)]
    public string? Name { get; set; }
}
