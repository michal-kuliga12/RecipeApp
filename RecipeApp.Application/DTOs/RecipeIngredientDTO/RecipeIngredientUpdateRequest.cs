using System.ComponentModel.DataAnnotations;
using RecipeApp.Domain.Enums;

namespace RecipeApp.Application.DTOs.RecipeIngredientDTO;

public class RecipeIngredientUpdateRequest
{
    [Required(ErrorMessage = "RecipeIngredientID jest wymagane")]
    public Guid? ID { get; set; }
    [Required(ErrorMessage = "IngredientID jest wymagany")]
    public Guid? IngredientID { get; set; }
    [Required(ErrorMessage = "RecipeID jest wymagany")]
    public Guid? RecipeID { get; set; }

    [Range(0.1, 10000, ErrorMessage = "Ilość musi być większa niż 0 i mniejsza niż 10 000")]
    public double? Quantity { get; set; }
    [Required(ErrorMessage = "Jednostka jest wymagana")]
    public Unit? Unit { get; set; }
}
