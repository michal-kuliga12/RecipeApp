using RecipeApp.Domain.Entities;
using RecipeApp.Domain.Entities.Enums;

namespace RecipeApp.Application.DTOs.RecipeDTO;

public class RecipeResponse
{
    public Guid ID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Author { get; set; }
    public Category Category { get; set; }

    public int PreparationTime { get; set; }
    public List<RecipeIngredient>? RecipeIngredients { get; set; }
    public int Servings { get; set; }
    public double Rating { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    // Dodaje nadpisywanie metody Equals aby sprawdzać kiedy obiekty są sobie równe
    public override bool Equals(object? obj)
    {
        if (obj is not RecipeResponse other) return false;
        return ID == other.ID; // albo wszystkie pola, które mają się liczyć
    }

    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }
}

public static class RecipeExtensions
{
    public static RecipeResponse ToRecipeResponse(this Recipe recipe)
        => new RecipeResponse()
        {
            ID = recipe.ID,
            Name = recipe.Name,
            Description = recipe.Description,
            Author = recipe.Author,
            Category = recipe.Category,
            PreparationTime = recipe.PreparationTime,
            RecipeIngredients = recipe.RecipeIngredients,
            Servings = recipe.Servings,
            Rating = recipe.Rating,
            ImageUrl = recipe.ImageUrl,
            CreatedAt = recipe.CreatedAt,
        };
}


