using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Domain.Enums;

public enum PrimaryCategory
{
    [Display(Name = "Śniadanie")]
    Breakfast,
    [Display(Name = "Danie główne")]
    MainCourse,
    [Display(Name = "Kolacja")]
    Dinner,
    [Display(Name = "Deser")]
    Dessert,
    [Display(Name = "Przekąska")]
    Snack,
    [Display(Name = "Napoje")]
    Drink
}
