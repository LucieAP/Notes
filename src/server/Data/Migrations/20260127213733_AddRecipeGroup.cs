using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RecipeGroupId",
                table: "Recipes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RecipeGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeGroups_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_RecipeGroupId",
                table: "Recipes",
                column: "RecipeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeGroups_CreatedBy",
                table: "RecipeGroups",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_RecipeGroups_RecipeGroupId",
                table: "Recipes",
                column: "RecipeGroupId",
                principalTable: "RecipeGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_RecipeGroups_RecipeGroupId",
                table: "Recipes");

            migrationBuilder.DropTable(
                name: "RecipeGroups");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_RecipeGroupId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "RecipeGroupId",
                table: "Recipes");
        }
    }
}
