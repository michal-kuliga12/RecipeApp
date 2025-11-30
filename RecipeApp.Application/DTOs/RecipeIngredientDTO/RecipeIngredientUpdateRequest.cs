using System.ComponentModel.DataAnnotations;
using RecipeApp.Core.Domain.Enums;
using RecipeApp.Core.Domain.Validation;

namespace RecipeApp.Core.DTOs.RecipeIngredientDTO;

public class RecipeIngredientUpdateRequest
{
    [NotEmptyGuid(ErrorMessage = "ID nie może być puste")]
    public Guid? ID { get; set; }
    [NotEmptyGuid(ErrorMessage = "ID nie może być puste")]
    public Guid? IngredientID { get; set; }
    [NotEmptyGuid(ErrorMessage = "ID nie może być puste")]
    public Guid? RecipeID { get; set; }

    [Range(0.1, 10000, ErrorMessage = "Ilość musi być większa niż 0 i mniejsza niż 10 000")]
    public double? Quantity { get; set; }
    [Required(ErrorMessage = "Jednostka jest wymagana")]
    public Unit? Unit { get; set; }
}
