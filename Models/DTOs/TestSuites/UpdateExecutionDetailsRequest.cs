namespace TestCaseManagement.Api.Models.DTOs.TestSuites;

public class UpdateExecutionDetailsRequest
{
    public string Result { get; set; } = "Pending";
    public string? Actual { get; set; }
    public string? Remarks { get; set; }
}