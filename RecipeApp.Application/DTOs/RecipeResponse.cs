using Entities;
using RecipeApp.Domain.Entities;
using RecipeApp.Domain.Enums;

namespace RecipeApp.Application.DTOs;

public class RecipeResponse
{
    public Guid ID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Author { get; set; }
    public PrimaryCategory PrimaryCategory { get; set; }

    public DishType DishType { get; set; }
    public int PreparationTime { get; set; }
    public List<RecipeIngredient>? RecipeIngredients { get; set; }
    public int Servings { get; set; }
    public double Rating { get; set; }
    public string? ImageUrl { get; set; }
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
            PrimaryCategory = recipe.PrimaryCategory,
            DishType = recipe.DishType,
            PreparationTime = recipe.PreparationTime,

            Servings = recipe.Servings,
            CreatedAt = recipe.CreatedAt,
        };
}
