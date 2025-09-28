using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Domain.Enums;

public enum Unit
{
    [Display(Name = "g")]
    Gram,

    [Display(Name = "kg")]
    Kilogram,

    [Display(Name = "ml")]
    Milliliter,

    [Display(Name = "l")]
    Liter,

    [Display(Name = "Łyżka")]
    Spoon,

    [Display(Name = "Łyżeczka")]
    Teaspoon,

    [Display(Name = "Sztuka")]
    Piece
}
