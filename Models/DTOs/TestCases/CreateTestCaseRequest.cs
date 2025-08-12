using System.Collections.Generic;

namespace TestCaseManagement.Api.Models.DTOs.TestCases
{
    public class CreateTestCaseRequest
    {
        public string ModuleId { get; set; } = string.Empty;

        // Added for version FK reference
        public string ProductVersionId { get; set; } = string.Empty;

        public string TestCaseId { get; set; } = string.Empty;
        public string UseCase { get; set; } = string.Empty;
        public string Scenario { get; set; } = string.Empty;
        public string TestType { get; set; } = "Manual"; // Default to Manual
        public string? TestTool { get; set; }
        public List<ManualTestCaseStepRequest> Steps { get; set; } = new();
    }

    
}
