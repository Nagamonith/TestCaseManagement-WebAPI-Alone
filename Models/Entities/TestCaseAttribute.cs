namespace TestCaseManagement.Api.Models.Entities;

public class TestCaseAttribute
{
    public string TestCaseId { get; set; } = string.Empty;
    public string ModuleAttributeId { get; set; } = string.Empty; // Now required
    public string Value { get; set; } = string.Empty;

    // Navigation properties
    public TestCase TestCase { get; set; } = null!;
    public ModuleAttribute ModuleAttribute { get; set; } = null!;
}