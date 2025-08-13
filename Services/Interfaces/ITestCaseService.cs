using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.TestCases;

namespace TestCaseManagement.Services.Interfaces;

public interface ITestCaseService
{
    Task<IEnumerable<TestCaseResponse>> GetAllTestCasesAsync(string moduleId);
    Task<TestCaseDetailResponse?> GetTestCaseByIdAsync(string moduleId, string id);
    Task<IdResponse> CreateTestCaseAsync(CreateTestCaseRequest request);
    Task<bool> UpdateTestCaseAsync(string moduleId, string id, UpdateTestCaseRequest request);
    Task<bool> DeleteTestCaseAsync(string moduleId, string id);
    Task<IEnumerable<TestCaseAttributeResponse>> GetTestCaseAttributesAsync(string moduleId, string testCaseId);
    Task<bool> UpdateTestCaseAttributesAsync(string moduleId, string testCaseId,
        IEnumerable<TestCaseAttributeRequest> attributes);
    Task AddTestCaseAttributeAsync(string moduleId, string testCaseId, TestCaseAttributeRequest request);

    Task<bool> UpdateTestCaseAttributeAsync(string moduleId, string testCaseId, string key, TestCaseAttributeRequest request);

    Task<bool> DeleteTestCaseAttributeAsync(string moduleId, string testCaseId, string key);


}