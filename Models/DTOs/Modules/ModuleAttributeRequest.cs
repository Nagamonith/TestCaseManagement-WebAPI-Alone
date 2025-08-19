namespace TestCaseManagement.Api.Models.DTOs.Modules;

public class ModuleAttributeRequest
{
   
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    
}