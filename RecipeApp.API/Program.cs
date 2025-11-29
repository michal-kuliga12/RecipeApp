using Microsoft.EntityFrameworkCore;
using RecipeApp.Application.Interfaces;
using RecipeApp.Application.Interfaces.RecipeInterfaces;
using RecipeApp.Application.RecipeServices;
using RecipeApp.Application.Services;
using RecipeApp.Infrastructure;
using RecipeApp.Infrastructure.Repositories;

namespace RecipeApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
            builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IRecipeService, RecipeService>();
            builder.Services.AddScoped<IRecipeCommandService, RecipeCommandService>();
            builder.Services.AddScoped<IRecipeDeleteService, RecipeDeleteService>();
            builder.Services.AddScoped<IRecipeFilterService, RecipeFilterService>();
            builder.Services.AddScoped<IRecipeQueryService, RecipeQueryService>();

            builder.Services.AddScoped<IIngredientService, IngredientService>();
            builder.Services.AddScoped<IRecipeIngredientService, RecipeIngredientService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
