using System.ComponentModel.DataAnnotations;
using RecipeApp.Core.Domain.Entities;
using RecipeApp.Core.Domain.Enums;

namespace RecipeApp.Core.DTOs.RecipeDTO;

public class RecipeAddRequest
{
    [Required(ErrorMessage = "Nazwa przepisu jest wymagana")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Nazwa musi mieć od 3 do 50 znaków")]
    public string? Name { get; set; }
    [StringLength(1000, ErrorMessage = "Opis nie może być dłuższy niż 1000 znaków")]
    public string? Description { get; set; }
    [Required(ErrorMessage = "Autor jest wymagany")]
    [StringLength(100, ErrorMessage = "Autor nie może być dłuższy niż 50 znaków")]
    public string? Author { get; set; }
    [Required(ErrorMessage = "Kategoria głowna jest wymagana")]
    public Category Category { get; set; }

    public int PreparationTime { get; set; }
    [Required(ErrorMessage = "Przepis musi zawierać co najmniej jeden składnik")]
    public List<RecipeIngredient>? RecipeIngredients { get; set; }
    [Range(1, 20, ErrorMessage = "Liczba porcji musi być w zakresie od 1 do 20")]
    public int Servings { get; set; } = 1;
    [Range(0.0, 5.0, ErrorMessage = "Ocena musi być w zakresie od 0 do 5")]
    public double Rating { get; set; }
    [StringLength(100, ErrorMessage = "Url jest za długi")]
    public string? ImageUrl { get; set; }
    [Required(ErrorMessage = "Data utworzenia jest wymagana")]
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }

    public Recipe ToRecipe()
        => new Recipe()
        {
            Name = Name,
            Description = Description,
            Author = Author,
            Category = Category,
            PreparationTime = PreparationTime,
            RecipeIngredients = RecipeIngredients,
            Servings = Servings,
            Rating = Rating,
            ImageUrl = ImageUrl,
            CreatedAt = CreatedAt
        };
}


