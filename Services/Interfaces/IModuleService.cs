using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.Modules;

namespace TestCaseManagement.Services.Interfaces;

public interface IModuleService
{
    Task<IEnumerable<ModuleResponse>> GetAllModulesAsync(string productId);
    Task<ModuleResponse?> GetModuleByIdAsync(string productId, string id);
    Task<IdResponse> CreateModuleAsync(CreateModuleRequest request);
    Task<bool> UpdateModuleAsync(string id, CreateModuleRequest request);
    Task<bool> DeleteModuleAsync(string productId, string id);
}