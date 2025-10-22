using System.ComponentModel.DataAnnotations;
using RecipeApp.Domain.Entities;
using RecipeApp.Domain.Enums;
using RecipeApp.Domain.Validation;

namespace RecipeApp.Application.DTOs.RecipeIngredientDTO;

public class RecipeIngredientAddRequest
{
    [NotEmptyGuid(ErrorMessage = "ID nie może być puste")]
    public Guid? IngredientID { get; set; }
    [StringLength(maximumLength: 50, ErrorMessage = "Nazwa składnika nie może być dłuższa niż 50 znaków")]
    public string? IngredientName { get; set; }
    [NotEmptyGuid(ErrorMessage = "ID nie może być puste")]
    public Guid? RecipeID { get; set; }
    [Range(0.1, 10000, ErrorMessage = "Ilość musi być większa niż 0 i mniejsza niż 10 000")]
    public double? Quantity { get; set; }
    [Required(ErrorMessage = "Jednostka jest wymagana")]
    public Unit? Unit { get; set; }

    public RecipeIngredient ToRecipeIngredient()
    {
        if (IngredientID == null)
            throw new InvalidOperationException("IngredientID nie może być null");
        if (RecipeID == null)
            throw new InvalidOperationException("RecipeID nie może być null");

        return new RecipeIngredient()
        {
            IngredientID = IngredientID.Value,
            RecipeID = RecipeID.Value,
            Quantity = Quantity ?? 1,
            Unit = Unit ?? Domain.Enums.Unit.Gram
        };
    }

}
