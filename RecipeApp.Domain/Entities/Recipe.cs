using System.ComponentModel.DataAnnotations;
using RecipeApp.Domain.Entities;
using RecipeApp.Domain.Enums;

namespace Entities;

public class Recipe
{
    [Key]
    public Guid ID { get; set; }
    [Required(ErrorMessage = "Nazwa przepisu jest wymagana")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Nazwa musi mieć od 3 do 50 znaków")]
    public string? Name { get; set; }
    [StringLength(1000, ErrorMessage = "Opis nie może być dłuższy niż 1000 znaków")]
    public string? Description { get; set; }
    [Required(ErrorMessage = "Autor jest wymagany")]
    [StringLength(100, ErrorMessage = "Autor nie może być dłuższy niż 50 znaków")]
    public string? Author { get; set; }
    [Required(ErrorMessage = "Kategoria głowna jest wymagana")]
    public PrimaryCategory PrimaryCategory { get; set; }

    [Required(ErrorMessage = "Typ dania jest wymagany")]
    public DishType DishType { get; set; }
    [Required(ErrorMessage = "Kategoria jest wymagana")]
    [StringLength(50, ErrorMessage = "Kategoria nie może być dłuższa niż 50 znaków")]
    public int PreparationTime { get; set; }
    [Required(ErrorMessage = "Przepis musi zawierać co najmniej jeden składnik")]
    public List<RecipeIngredient>? RecipeIngredients { get; set; }
    [Range(1, 20, ErrorMessage = "Liczba porcji musi być w zakresie od 1 do 20")]
    public int Servings { get; set; }
    [Range(0.0, 5.0, ErrorMessage = "Ocena musi być w zakresie od 0 do 5")]
    public double Rating { get; set; }
    [Url(ErrorMessage = "Niepoprawny adres URL obrazu")]
    public string? ImageUrl { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }

    #region Dodatkowe pomysly
    //  Tags(lista tagów, np. "vege", "bez glutenu").
    //CookTime(czas gotowania osobno od przygotowania)
    //DifficultyLevel(np.łatwe / średnie / trudne).
    //Likes/Favorites(ile osób dodało do ulubionych).
    //Comments(powiązana tabela z opiniami użytkowników).
    #endregion
}
