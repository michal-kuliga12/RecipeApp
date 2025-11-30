namespace RecipeApp.Core.ServicesContracts.RecipeContracts;

public interface IRecipeService :
    IRecipeCommandService,
    IRecipeQueryService,
    IRecipeFilterService,
    IRecipeDeleteService
{ }


