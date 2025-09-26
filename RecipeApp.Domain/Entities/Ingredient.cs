namespace Entities;

public class Ingredient
{
    public int ID { get; set; }
    public string Name { get; set; }            // np. "mąka"
    public double Quantity { get; set; }        // np. 200
    public string Unit { get; set; }            // np. "g", "ml", "łyżka"
    public int RecipeId { get; set; }           // klucz obcy -> Recipe
}
