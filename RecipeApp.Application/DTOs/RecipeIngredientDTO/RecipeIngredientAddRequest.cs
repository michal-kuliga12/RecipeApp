using System.ComponentModel.DataAnnotations;
using RecipeApp.Domain.Entities;
using RecipeApp.Domain.Entities.Enums;
using RecipeApp.Domain.Entities.Validation;

namespace RecipeApp.Application.DTOs.RecipeIngredientDTO;

[RequireIDorString("IngredientID", "IngredientName")]
public class RecipeIngredientAddRequest
{
    public Guid IngredientID { get; set; }
    [StringLength(maximumLength: 50, ErrorMessage = "Nazwa składnika nie może być dłuższa niż 50 znaków")]
    public string? IngredientName { get; set; }
    [NotEmptyGuid(ErrorMessage = "ID nie może być puste")]
    public Guid RecipeID { get; set; }
    [Range(0.1, 10000, ErrorMessage = "Ilość musi być większa niż 0 i mniejsza niż 10 000")]
    public double? Quantity { get; set; }
    [Required(ErrorMessage = "Jednostka jest wymagana")]
    public Unit? Unit { get; set; }

    public RecipeIngredient ToRecipeIngredient()
    {
        return new RecipeIngredient()
        {
            IngredientID = IngredientID,
            RecipeID = RecipeID,
            Quantity = Quantity ?? 1,
            Unit = Unit ?? Domain.Entities.Enums.Unit.Gram
        };
    }

}
