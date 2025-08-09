using System;

namespace TestCaseManagement.Api.Models.DTOs.TestCases
{
    public class TestCaseResponse
    {
        public string Id { get; set; } = string.Empty;
        public string ModuleId { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string TestCaseId { get; set; } = string.Empty;
        public string UseCase { get; set; } = string.Empty;
        public string Scenario { get; set; } = string.Empty;
        public string TestType { get; set; } = string.Empty;
        public string? TestTool { get; set; }
        public string? Result { get; set; }
        public string? Actual { get; set; }  // Added
        public string? Remarks { get; set; } // Added
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}