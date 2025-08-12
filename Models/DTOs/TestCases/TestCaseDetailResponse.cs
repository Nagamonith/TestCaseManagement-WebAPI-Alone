using TestCaseManagement.Api.Models.DTOs.Uploads;

namespace TestCaseManagement.Api.Models.DTOs.TestCases
{
    public class TestCaseDetailResponse : TestCaseResponse
    {
        // Changed Steps and Expected to lists of strings instead of ManualTestCaseStepRequest objects
        public List<string> Steps { get; set; } = new();
        public List<string> Expected { get; set; } = new();

        public List<TestCaseAttributeResponse> Attributes { get; set; } = new();
        public List<UploadResponse> Attachments { get; set; } = new();
        public List<string> TestSuiteIds { get; set; } = new();
    }
}
