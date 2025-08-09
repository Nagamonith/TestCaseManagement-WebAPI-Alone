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
}