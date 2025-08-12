using ClosedXML.Excel;
using System.Data;
using System.Drawing;
using TestCaseManagement.Api.Models.DTOs.TestCases;

namespace TestCaseManagement.Api.Utilities
{
    public class ExcelExporter
    {
        public byte[] ExportTestCasesToExcel(IEnumerable<TestCaseResponse> testCases)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Test Cases");

            // Create headers
            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Test Case ID";
            worksheet.Cell(1, 3).Value = "Use Case";
            worksheet.Cell(1, 4).Value = "Scenario";
            worksheet.Cell(1, 5).Value = "Test Type";
            worksheet.Cell(1, 6).Value = "Result";

            // Style headers
            var headerRange = worksheet.Range(1, 1, 1, 6);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Add data
            int row = 2;
            foreach (var testCase in testCases)
            {
                worksheet.Cell(row, 1).Value = testCase.Id;
                worksheet.Cell(row, 2).Value = testCase.TestCaseId;
                worksheet.Cell(row, 3).Value = testCase.UseCase;
                worksheet.Cell(row, 4).Value = testCase.Scenario;
                worksheet.Cell(row, 5).Value = testCase.TestType;
                worksheet.Cell(row, 6).Value = testCase.Result;
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportDetailedTestCaseToExcel(TestCaseDetailResponse testCase)
        {
            using var workbook = new XLWorkbook();

            // Main info worksheet
            var mainSheet = workbook.Worksheets.Add("Test Case Details");
            mainSheet.Cell(1, 1).Value = "Field";
            mainSheet.Cell(1, 2).Value = "Value";

            mainSheet.Cell(2, 1).Value = "Test Case ID";
            mainSheet.Cell(2, 2).Value = testCase.TestCaseId;

            // Add more fields as needed...

            // Steps worksheet
            if (testCase.Steps.Any())
            {
                var stepsSheet = workbook.Worksheets.Add("Steps");
                stepsSheet.Cell(1, 1).Value = "Step";
                stepsSheet.Cell(1, 2).Value = "Expected Result";

                int row = 2;
                for (int i = 0; i < testCase.Steps.Count; i++)
                {
                    string stepText = testCase.Steps[i];
                    string expectedText = i < testCase.Expected.Count ? testCase.Expected[i] : string.Empty;

                    stepsSheet.Cell(row, 1).Value = stepText;
                    stepsSheet.Cell(row, 2).Value = expectedText;
                    row++;
                }

                stepsSheet.Columns().AdjustToContents();
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
