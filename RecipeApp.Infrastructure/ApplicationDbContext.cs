using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Domain.Entities;

namespace RecipeApp.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<Recipe> Recipes { get; set; }
    public virtual DbSet<Ingredient> Ingredients { get; set; }
    public virtual DbSet<RecipeIngredient> RecipeIngredients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var deseralizerOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        string ingredientsJson = File.ReadAllText("ingredients.json");
        List<Ingredient>? ingredients = JsonSerializer.Deserialize<List<Ingredient>>(ingredientsJson, deseralizerOptions);
        foreach (Ingredient ingredient in ingredients)
            modelBuilder.Entity<Ingredient>().HasData(ingredient);

        string recipesJson = File.ReadAllText("recipes.json");
        List<Recipe>? recipes = JsonSerializer.Deserialize<List<Recipe>>(recipesJson, deseralizerOptions);
        foreach (Recipe recipe in recipes)
            modelBuilder.Entity<Recipe>().HasData(recipe);
    }
}
