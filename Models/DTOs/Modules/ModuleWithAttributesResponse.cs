namespace TestCaseManagement.Api.Models.DTOs.Modules;

public class ModuleWithAttributesResponse : ModuleResponse
{
    public List<ModuleAttributeRequest> Attributes { get; set; } = new();
}