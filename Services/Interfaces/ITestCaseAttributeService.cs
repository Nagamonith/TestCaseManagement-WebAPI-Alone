using TestCaseManagement.Api.Models.DTOs.TestCases;

namespace TestCaseManagement.Services.Interfaces;

public interface ITestCaseAttributeService
{
    Task<IEnumerable<TestCaseAttributeResponse>> GetAllAttributesAsync(string testCaseId);
    Task AddAttributeAsync(string testCaseId, TestCaseAttributeRequest request);
    Task<bool> DeleteAttributeAsync(string testCaseId, string key);
}