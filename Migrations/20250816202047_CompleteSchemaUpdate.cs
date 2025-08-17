using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestCaseManagement.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class CompleteSchemaUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. First add all columns without foreign keys
            migrationBuilder.AddColumn<int>(
                name: "TestSuiteTestCaseId",
                table: "Uploads",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductVersionId",
                table: "TestSuiteTestCases",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Actual",
                table: "TestSuiteTestCases",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "TestSuiteTestCases",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "TestSuiteTestCases",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TestSuiteTestCases",
                type: "datetime2",
                nullable: true);

            // 2. Create index first
            migrationBuilder.CreateIndex(
                name: "IX_Uploads_TestSuiteTestCaseId",
                table: "Uploads",
                column: "TestSuiteTestCaseId");

            // 3. Add foreign key with NO ACTION to prevent cascade paths
            migrationBuilder.AddForeignKey(
                name: "FK_Uploads_TestSuiteTestCases_TestSuiteTestCaseId",
                table: "Uploads",
                column: "TestSuiteTestCaseId",
                principalTable: "TestSuiteTestCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction); // Changed from SetNull to NoAction
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. First drop the foreign key
            migrationBuilder.DropForeignKey(
                name: "FK_Uploads_TestSuiteTestCases_TestSuiteTestCaseId",
                table: "Uploads");

            // 2. Then drop the index
            migrationBuilder.DropIndex(
                name: "IX_Uploads_TestSuiteTestCaseId",
                table: "Uploads");

            // 3. Remove all added columns
            migrationBuilder.DropColumn(
                name: "TestSuiteTestCaseId",
                table: "Uploads");

            migrationBuilder.DropColumn(
                name: "Actual",
                table: "TestSuiteTestCases");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "TestSuiteTestCases");

            migrationBuilder.DropColumn(
                name: "Result",
                table: "TestSuiteTestCases");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TestSuiteTestCases");

            // 4. Restore original column definition
            migrationBuilder.AlterColumn<string>(
                name: "ProductVersionId",
                table: "TestSuiteTestCases",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}