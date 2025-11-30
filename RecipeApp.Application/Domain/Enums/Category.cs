using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Core.Domain.Enums;

public enum Category
{
    [Display(Name = "Śniadania")]
    Breakfast,

    [Display(Name = "Zupy")]
    Soups,

    [Display(Name = "Sałatki")]
    Salads,

    [Display(Name = "Wypieki i desery")]
    PastriesAndDesserts,

    [Display(Name = "Dania główne")]
    MainCourse,

    [Display(Name = "Dodatki")]
    SideDishes,

    [Display(Name = "Napoje")]
    Drinks,

    [Display(Name = "Przekąski")]
    Snacks
}
