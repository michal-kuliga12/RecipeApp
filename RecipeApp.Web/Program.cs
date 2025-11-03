using Microsoft.EntityFrameworkCore;
using RecipeApp.Application.Interfaces;
using RecipeApp.Application.Services;
using RecipeApp.Infrastructure;
using RecipeApp.Infrastructure.Repositories;

namespace CookBookApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddSingleton<IRecipeService, RecipeService>();


            var app = builder.Build();

            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();

            app.Run();
        }
    }
}
