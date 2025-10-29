using Microsoft.AspNetCore.Mvc;
using RecipeApp.Application.DTOs.RecipeDTO;
using RecipeApp.Application.Interfaces;

namespace RecipeApp.Web.Controllers;

[Route("[controller]")]
public class RecipeController : Controller
{
    private readonly IRecipeService _recipeService;

    public RecipeController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    [Route("/")]
    [Route("[action]")]
    public async Task<IActionResult> Index()
    {
        List<RecipeResponse>? recipes = await _recipeService.GetAllRecipes();
        return View(recipes);
    }

    [Route("[action]/{id:Guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        RecipeResponse? recipe = await _recipeService.GetRecipeByID(id);
        if (recipe == null)
            return NotFound("Nie znaleziono przepisu o danym ID");

        return View(recipe);
    }

    [Route("[action]")]
    [HttpGet]
    public IActionResult Create()
    {
        RecipeAddRequest recipeToAdd = new();
        return View(recipeToAdd);
    }

    [Route("[action]")]
    [HttpPost]
    public IActionResult Create(RecipeAddRequest recipeAddRequest)
    {
        return RedirectToAction("Index");
    }

}
