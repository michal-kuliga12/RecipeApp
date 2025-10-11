using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Domain.Entities;

public class Ingredient
{
    [Key]
    [Required]
    public Guid? ID { get; set; }
    [Required(ErrorMessage = "Nazwa produktu jest wymagana")]
    [Range(2, 50)]
    public string? Name { get; set; }
}
