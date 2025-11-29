namespace RecipeApp.Application.Interfaces.RecipeInterfaces;

public interface IRecipeService :
    IRecipeCommandService,
    IRecipeQueryService,
    IRecipeFilterService,
    IRecipeDeleteService
{ }


