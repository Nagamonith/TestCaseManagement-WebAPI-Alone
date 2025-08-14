using System;

namespace TestCaseManagement.Api.Models.DTOs.TestCases
{
    public class TestCaseResponse
    {
        public string Id { get; set; } = string.Empty;
        public string ModuleId { get; set; } = string.Empty;

        // Keep for backward compatibility if you are already using it
        
        // New field for actual ProductVersion.Name
        public string? ProductVersionName { get; set; }

        public string TestCaseId { get; set; } = string.Empty;
        public string UseCase { get; set; } = string.Empty;
        public string Scenario { get; set; } = string.Empty;
        public string TestType { get; set; } = string.Empty;
        public string? TestTool { get; set; }
        public string? Result { get; set; }
        public string? Actual { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
