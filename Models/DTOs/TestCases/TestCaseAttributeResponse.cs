namespace TestCaseManagement.Api.Models.DTOs.TestCases;

public class TestCaseAttributeResponse
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
}