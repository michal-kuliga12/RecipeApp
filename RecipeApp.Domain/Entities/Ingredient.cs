using System.ComponentModel.DataAnnotations;

namespace Entities;

public class Ingredient
{
    [Key]
    public Guid ID { get; set; }
    [Required(ErrorMessage = "Nazwa produktu jest wymagana")]
    [Range(2, 50)]
    public string? Name { get; set; }
}
