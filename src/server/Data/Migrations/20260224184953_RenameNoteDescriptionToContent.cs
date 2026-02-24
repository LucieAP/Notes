using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameNoteDescriptionToContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Notes");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Tasks",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Recipes",
                newName: "Content");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Notes",
                type: "character varying(65535)",
                maxLength: 65535,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Notes");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Tasks",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Recipes",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Notes",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: true);
        }
    }
}
