using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.Modules;

namespace TestCaseManagement.Services.Interfaces;

public interface IModuleAttributeService
{
    Task<IEnumerable<ModuleAttributeResponse>> GetAllAttributesAsync(string moduleId);

    Task<IdResponse> CreateAttributeAsync(string moduleId, ModuleAttributeRequest request);
    Task<bool> DeleteAttributeAsync(string moduleId, string id);

    Task<bool> UpdateAttributeAsync(string moduleId, string attributeId, ModuleAttributeRequest request);

}