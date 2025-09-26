using Entities;

namespace RecipeApp.Application.DTOs;

public class RecipeResponse
{
    public Guid ID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Author { get; set; }
    public string? Category { get; set; }
    public int PreparationTime { get; set; }
    public int Servings { get; set; }
    public DateTime CreatedAt { get; set; }
}

public static class RecipeExtensions
{
    public static RecipeResponse RecipeToRecipeResponse(this Recipe recipe)
        => new RecipeResponse()
        {
            ID = recipe.ID,
            Name = recipe.Name,
            Description = recipe.Description,
            Author = recipe.Author,
            Category = recipe.Category,
            PreparationTime = recipe.PreparationTime,
            Servings = recipe.Servings,
            CreatedAt = recipe.CreatedAt,
        };
}
