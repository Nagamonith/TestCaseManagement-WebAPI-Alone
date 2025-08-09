using TestCaseManagement.Api.Models.DTOs.TestCases;

namespace TestCaseManagement.Services.Interfaces;

public interface IManualTestCaseStepService
{
    Task<IEnumerable<ManualTestCaseStepRequest>> GetAllStepsAsync(string testCaseId);
    Task AddStepAsync(string testCaseId, ManualTestCaseStepRequest request);
    Task<bool> DeleteStepAsync(string testCaseId, int stepId);
}