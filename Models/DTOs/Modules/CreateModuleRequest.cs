namespace TestCaseManagement.Api.Models.DTOs.Modules;

public class CreateModuleRequest
{
    public string ProductId { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}