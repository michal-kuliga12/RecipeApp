using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RecipeApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Seeddb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Ingredients",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { new Guid("1a3f3d48-7b4a-4970-97cd-1ce6ac2e4deb"), "Oliwa z oliwek" },
                    { new Guid("2f0775fe-1aa3-4772-b4cf-f0c17c4d0e4b"), "Sól" },
                    { new Guid("4294f87a-d8c5-4005-a315-11d592a74c09"), "Jajka" },
                    { new Guid("5528f110-0c49-4e76-a746-c6e6fabc5d99"), "Mąka pszenna" },
                    { new Guid("6d71d442-8855-423e-9fa9-dfc71c41ef95"), "Drożdże" },
                    { new Guid("7d217348-9187-4521-9da6-f185d515f051"), "Masło" },
                    { new Guid("7e52f8fc-5714-4e53-b6c1-c227bba079b4"), "Czosnek" },
                    { new Guid("86c041f0-185e-4050-be1c-1ce7ba5cd070"), "Mleko" },
                    { new Guid("900d93d4-bf2f-4cd1-8eb4-4549d2b17622"), "Cukier" },
                    { new Guid("af9f9c52-4cb9-405c-983f-0ba5f57f79ec"), "Cebula" }
                });

            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "ID", "Author", "Category", "CreatedAt", "Description", "ImageUrl", "Name", "PreparationTime", "Rating", "Servings" },
                values: new object[,]
                {
                    { new Guid("a6c1e7e1-7d43-4b45-9f8e-2b74b3a0f1d2"), "Jan Kowalski", 0, new DateTime(2025, 10, 25, 10, 0, 0, 0, DateTimeKind.Utc), "Tradycyjne cienkie naleśniki z mąki, mleka i jajek, idealne do słodkich dodatków.", "https://cdn.pixabay.com/photo/2016/10/25/12/28/pancakes-1763756_1280.jpg", "Naleśniki klasyczne", 20, 4.5, 4 },
                    { new Guid("b7125d3f-7f6d-4ac4-9f9e-98e022bca3f4"), "Maria Papadopoulos", 2, new DateTime(2025, 10, 25, 10, 30, 0, 0, DateTimeKind.Utc), "Świeża sałatka z pomidorami, ogórkiem, serem feta i oliwkami.", "https://cdn.pixabay.com/photo/2017/06/05/17/29/salad-2380329_1280.jpg", "Sałatka grecka", 15, 4.5999999999999996, 2 },
                    { new Guid("c4f5b3e4-bc11-42d1-8e5f-4fd8a788fa6e"), "Luca Rossi", 4, new DateTime(2025, 10, 25, 10, 20, 0, 0, DateTimeKind.Utc), "Klasyczne włoskie danie z makaronu, oliwy, czosnku i chili.", "https://cdn.pixabay.com/photo/2017/07/16/10/43/spaghetti-2508184_1280.jpg", "Spaghetti aglio e olio", 25, 4.7000000000000002, 2 },
                    { new Guid("d2c8e8ac-054d-48b7-a893-98c1b49f7c21"), "Anna Nowak", 1, new DateTime(2025, 10, 25, 10, 10, 0, 0, DateTimeKind.Utc), "Aksamitna zupa z pieczonej dyni, z dodatkiem imbiru i śmietanki.", "https://cdn.pixabay.com/photo/2017/10/24/23/12/pumpkin-soup-2885151_1280.jpg", "Zupa krem z dyni", 45, 4.7999999999999998, 3 },
                    { new Guid("e32dbe67-cb17-4962-bc9d-b3bb3c1f10af"), "Julia Nowak", 3, new DateTime(2025, 10, 25, 10, 40, 0, 0, DateTimeKind.Utc), "Intensywnie czekoladowe, wilgotne brownie z chrupiącą skórką.", "https://cdn.pixabay.com/photo/2018/06/09/16/00/brownie-3461511_1280.jpg", "Brownie czekoladowe", 50, 4.9000000000000004, 6 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "ID",
                keyValue: new Guid("1a3f3d48-7b4a-4970-97cd-1ce6ac2e4deb"));

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "ID",
                keyValue: new Guid("2f0775fe-1aa3-4772-b4cf-f0c17c4d0e4b"));

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "ID",
                keyValue: new Guid("4294f87a-d8c5-4005-a315-11d592a74c09"));

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "ID",
                keyValue: new Guid("5528f110-0c49-4e76-a746-c6e6fabc5d99"));

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "ID",
                keyValue: new Guid("6d71d442-8855-423e-9fa9-dfc71c41ef95"));

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "ID",
                keyValue: new Guid("7d217348-9187-4521-9da6-f185d515f051"));

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "ID",
                keyValue: new Guid("7e52f8fc-5714-4e53-b6c1-c227bba079b4"));

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "ID",
                keyValue: new Guid("86c041f0-185e-4050-be1c-1ce7ba5cd070"));

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "ID",
                keyValue: new Guid("900d93d4-bf2f-4cd1-8eb4-4549d2b17622"));

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "ID",
                keyValue: new Guid("af9f9c52-4cb9-405c-983f-0ba5f57f79ec"));

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "ID",
                keyValue: new Guid("a6c1e7e1-7d43-4b45-9f8e-2b74b3a0f1d2"));

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "ID",
                keyValue: new Guid("b7125d3f-7f6d-4ac4-9f9e-98e022bca3f4"));

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "ID",
                keyValue: new Guid("c4f5b3e4-bc11-42d1-8e5f-4fd8a788fa6e"));

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "ID",
                keyValue: new Guid("d2c8e8ac-054d-48b7-a893-98c1b49f7c21"));

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "ID",
                keyValue: new Guid("e32dbe67-cb17-4962-bc9d-b3bb3c1f10af"));
        }
    }
}
