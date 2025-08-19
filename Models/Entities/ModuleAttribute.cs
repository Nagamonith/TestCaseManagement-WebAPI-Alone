namespace TestCaseManagement.Api.Models.Entities;

public class ModuleAttribute
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ModuleId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    

    public Module Module { get; set; } = null!;

    // New: test-case attributes referencing this module attribute
    public ICollection<TestCaseAttribute> TestCaseAttributes { get; set; } = new List<TestCaseAttribute>();
}
