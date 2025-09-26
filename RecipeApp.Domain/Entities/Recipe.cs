namespace Entities;

public class Recipe
{
    public Guid ID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Author { get; set; }
    public string? Category { get; set; }
    public int PreparationTime { get; set; }
    public List<Ingredient>? Ingredients { get; set; }
    public int Servings { get; set; }
    public double Rating { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    #region Dodatkowe pomysly
    //  Tags(lista tagów, np. "vege", "bez glutenu").
    //CookTime(czas gotowania osobno od przygotowania)
    //DifficultyLevel(np.łatwe / średnie / trudne).
    //Likes/Favorites(ile osób dodało do ulubionych).
    //Comments(powiązana tabela z opiniami użytkowników).
    #endregion
}
