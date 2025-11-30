using System.ComponentModel.DataAnnotations;
using RecipeApp.Domain.Entities.Enums;

namespace RecipeApp.Domain.Entities;

public class Recipe
{
    [Key]
    public Guid ID { get; set; }
    [Required(ErrorMessage = "Nazwa przepisu jest wymagana")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Nazwa musi mieć od 3 do 50 znaków")]
    public required string Name { get; set; }
    [StringLength(1000, ErrorMessage = "Opis nie może być dłuższy niż 1000 znaków")]
    public string? Description { get; set; }
    [Required(ErrorMessage = "Autor jest wymagany")]
    [StringLength(100, ErrorMessage = "Autor nie może być dłuższy niż 50 znaków")]
    public required string Author { get; set; }
    [Required(ErrorMessage = "Kategoria głowna jest wymagana")]
    public required Category Category { get; set; }

    [Required(ErrorMessage = "Kategoria jest wymagana")]
    [Range(1, 1440)]
    public int PreparationTime { get; set; }
    [Required(ErrorMessage = "Przepis musi zawierać co najmniej jeden składnik")]
    public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
    [Range(1, 20, ErrorMessage = "Liczba porcji musi być w zakresie od 1 do 20")]
    public int Servings { get; set; }
    [Range(0.0, 5.0, ErrorMessage = "Ocena musi być w zakresie od 0 do 5")]
    public double Rating { get; set; }
    [StringLength(100, ErrorMessage = "Url jest za długi")]
    public string? ImageUrl { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

#region Dodatkowe pomysly
//  Tags(lista tagów, np. "vege", "bez glutenu").
//CookTime(czas gotowania osobno od przygotowania)
//DifficultyLevel(np.łatwe / średnie / trudne).
//Likes/Favorites(ile osób dodało do ulubionych).
//Comments(powiązana tabela z opiniami użytkowników).
#endregion
