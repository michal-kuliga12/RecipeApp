using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Core.Domain.Entities;

public class Ingredient
{
    [Key]
    public Guid ID { get; set; }
    [Required(ErrorMessage = "Nazwa produktu jest wymagana")]
    [StringLength(50, MinimumLength = 2)]
    public required string Name { get; set; }
}
