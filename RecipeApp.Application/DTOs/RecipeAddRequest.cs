using Entities;

namespace RecipeApp.Application.DTOs;

public class RecipeAddRequest
{
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

    Recipe ToRecipe()
        => new Recipe()
        {
            Name = Name,
            Description = Description,
            Author = Author,
            Category = Category,
            PreparationTime = PreparationTime,
            Ingredients = Ingredients,
            Servings = Servings,
            Rating = Rating,
            ImageUrl = ImageUrl,
            CreatedAt = CreatedAt
        };
}


