using Microsoft.AspNetCore.Mvc;
using RecipeApp.Application.Interfaces;
using RecipeApp.Infrastructure;

namespace RecipeApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecipesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IRecipeService _recipeService;

    public RecipesController(ApplicationDbContext context, IRecipeService recipeService)
    {
        _context = context;
        _recipeService = recipeService;
    }

    // DO POPRAWIENIA 
    /* 

    // GET: api/Recipes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecipeResponse?>>> GetRecipes()
    {
        //return await _recipeService.GetAllRecipes();
    }

    // GET: api/Recipes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RecipeResponse?>> GetRecipe(Guid id)
    {
        var recipe = await _recipeService.GetRecipeByID(id);

        if (recipe == null)
        {
            return NotFound();
        }

        return recipe;
    }

    // PUT: api/Recipes/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutRecipe(Guid id, RecipeUpdateRequest recipe)
    {
        if (id != recipe.ID)
        {
            return BadRequest();
        }

        try
        {
            RecipeResponse? response = await _recipeService.UpdateRecipe(recipe);
            if (response is null)
            {
                return NotFound();
            }
        }
        catch (ArgumentNullException)
        {
            return BadRequest();
        }

        return NoContent();
    }

    // POST: api/Recipes
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Recipe>> PostRecipe(RecipeAddRequest recipe)
    {
        try
        {
            RecipeResponse? recipeAdded = await _recipeService.AddRecipe(recipe);
            return CreatedAtAction("GetRecipe", new { ID = recipeAdded.ID }, recipeAdded);
        }
        catch (ArgumentNullException)
        {
            return BadRequest();
        }
    }

    // DELETE: api/Recipes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(Guid id)
    {
        try
        {
            bool isDeleted = await _recipeService.DeleteRecipe(id);
            if (!isDeleted)
            {
                return NotFound();
            }
        }
        catch (ArgumentNullException)
        {
            return BadRequest();
        }

        return NoContent();
    }
    */
}
