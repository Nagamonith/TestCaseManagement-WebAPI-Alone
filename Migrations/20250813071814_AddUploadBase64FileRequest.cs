using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestCaseManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUploadBase64FileRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Uploads_TestCases_TestCaseId",
                table: "Uploads");

            migrationBuilder.AddForeignKey(
                name: "FK_Uploads_TestCases_TestCaseId",
                table: "Uploads",
                column: "TestCaseId",
                principalTable: "TestCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Uploads_TestCases_TestCaseId",
                table: "Uploads");

            migrationBuilder.AddForeignKey(
                name: "FK_Uploads_TestCases_TestCaseId",
                table: "Uploads",
                column: "TestCaseId",
                principalTable: "TestCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
