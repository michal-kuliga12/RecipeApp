using System.ComponentModel.DataAnnotations;
using RecipeApp.Domain.Entities;

namespace RecipeApp.Application.DTOs.IngredientDTO;

public class IngredientResponse
{

    [Required]
    public Guid ID { get; set; }
    [Required(ErrorMessage = "Nazwa produktu jest wymagana")]
    [Range(2, 50)]
    public string? Name { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not Ingredient other)
            return false;

        return ID == other.ID && Name == other.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID, Name);
    }
}
