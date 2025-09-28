using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Domain.Enums;

public enum DishType
{
    [Display(Name = "Zupa")]
    Soup,
    [Display(Name = "Sałatka")]
    Salad,
    [Display(Name = "Danie główne")]
    MainDish,
    [Display(Name = "Dodatki / przystawki")]
    SideDish,
    [Display(Name = "Sos / dip")]
    Sauce,
    [Display(Name = "Wypiek")]
    BakedGood
}
