using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestCaseManagement.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class YourMigrationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TestCaseAttributes",
                table: "TestCaseAttributes");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "TestCaseAttributes");

            migrationBuilder.AddColumn<string>(
                name: "ModuleAttributeId",
                table: "TestCaseAttributes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestCaseAttributes",
                table: "TestCaseAttributes",
                columns: new[] { "TestCaseId", "ModuleAttributeId" });

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseAttributes_ModuleAttributeId",
                table: "TestCaseAttributes",
                column: "ModuleAttributeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCaseAttributes_ModuleAttributes_ModuleAttributeId",
                table: "TestCaseAttributes",
                column: "ModuleAttributeId",
                principalTable: "ModuleAttributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCaseAttributes_ModuleAttributes_ModuleAttributeId",
                table: "TestCaseAttributes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestCaseAttributes",
                table: "TestCaseAttributes");

            migrationBuilder.DropIndex(
                name: "IX_TestCaseAttributes_ModuleAttributeId",
                table: "TestCaseAttributes");

            migrationBuilder.DropColumn(
                name: "ModuleAttributeId",
                table: "TestCaseAttributes");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "TestCaseAttributes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestCaseAttributes",
                table: "TestCaseAttributes",
                columns: new[] { "TestCaseId", "Key" });
        }
    }
}
