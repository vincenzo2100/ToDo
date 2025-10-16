using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ToDo.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TDTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CompletionPercentage = table.Column<double>(type: "double precision", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TDTasks", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TDTasks",
                columns: new[] { "Id", "CompletionPercentage", "Description", "ExpirationDate", "Status", "Title" },
                values: new object[,]
                {
                    { 1, 30.0, "Description1", new DateTime(2025, 10, 16, 0, 0, 0, 0, DateTimeKind.Utc), "InProgress", "Task1" },
                    { 2, 40.0, "Description2", new DateTime(2025, 10, 16, 0, 0, 0, 0, DateTimeKind.Utc), "InProgress", "Task2" },
                    { 3, 50.0, "Description3", new DateTime(2025, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), "InProgress", "Task3" },
                    { 4, 60.0, "Description4", new DateTime(2025, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), "InProgress", "Task4" },
                    { 5, 70.0, "Description5", new DateTime(2025, 10, 19, 0, 0, 0, 0, DateTimeKind.Utc), "InProgress", "Task5" },
                    { 6, 80.0, "Description6", new DateTime(2025, 10, 19, 0, 0, 0, 0, DateTimeKind.Utc), "InProgress", "Task6" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TDTasks");
        }
    }
}
