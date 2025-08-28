using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestCaseManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTestSuiteTestCaseService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestRunResults_TestCases_TestCaseId",
                table: "TestRunResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSuites_Products_ProductId",
                table: "TestSuites");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSuiteTestCases_ProductVersions_ProductVersionId",
                table: "TestSuiteTestCases");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSuiteTestCases_TestSuites_TestSuiteId",
                table: "TestSuiteTestCases");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRunResults_TestCases_TestCaseId",
                table: "TestRunResults",
                column: "TestCaseId",
                principalTable: "TestCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestSuites_Products_ProductId",
                table: "TestSuites",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestSuiteTestCases_ProductVersions_ProductVersionId",
                table: "TestSuiteTestCases",
                column: "ProductVersionId",
                principalTable: "ProductVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TestSuiteTestCases_TestSuites_TestSuiteId",
                table: "TestSuiteTestCases",
                column: "TestSuiteId",
                principalTable: "TestSuites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestRunResults_TestCases_TestCaseId",
                table: "TestRunResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSuites_Products_ProductId",
                table: "TestSuites");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSuiteTestCases_ProductVersions_ProductVersionId",
                table: "TestSuiteTestCases");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSuiteTestCases_TestSuites_TestSuiteId",
                table: "TestSuiteTestCases");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRunResults_TestCases_TestCaseId",
                table: "TestRunResults",
                column: "TestCaseId",
                principalTable: "TestCases",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestSuites_Products_ProductId",
                table: "TestSuites",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestSuiteTestCases_ProductVersions_ProductVersionId",
                table: "TestSuiteTestCases",
                column: "ProductVersionId",
                principalTable: "ProductVersions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestSuiteTestCases_TestSuites_TestSuiteId",
                table: "TestSuiteTestCases",
                column: "TestSuiteId",
                principalTable: "TestSuites",
                principalColumn: "Id");
        }
    }
}
