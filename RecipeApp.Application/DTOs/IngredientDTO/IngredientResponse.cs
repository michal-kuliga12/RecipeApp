using System.ComponentModel.DataAnnotations;
using RecipeApp.Core.Domain.Entities;

namespace RecipeApp.Core.DTOs.IngredientDTO;

public class IngredientResponse
{

    [Required]
    public Guid ID { get; set; }
    [Required(ErrorMessage = "Nazwa produktu jest wymagana")]
    [StringLength(50, MinimumLength = 2)]
    public string? Name { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not IngredientResponse other) return false;
        return ID == other.ID && Name == other.Name; // albo wszystkie pola, które mają się liczyć
    }

    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }
}

public static class IngredientExtension
{
    public static IngredientResponse ToIngredientResponse(this Ingredient ingredient)
        => new IngredientResponse()
        {
            ID = ingredient.ID,
            Name = ingredient.Name
        };
}
