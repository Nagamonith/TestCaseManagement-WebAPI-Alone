using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestCaseManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOptionsFromModuleAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Options",
                table: "ModuleAttributes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Options",
                table: "ModuleAttributes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
