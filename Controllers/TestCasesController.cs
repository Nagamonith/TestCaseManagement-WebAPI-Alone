using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.TestCases;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/modules/{moduleId}/testcases")]
public class TestCasesController : ControllerBase
{
    private readonly ITestCaseService _testCaseService;
    private readonly ILogger<TestCasesController> _logger;

    public TestCasesController(ITestCaseService testCaseService, ILogger<TestCasesController> logger)
    {
        _testCaseService = testCaseService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestCaseResponse>>> GetAll(string moduleId)
    {
        var testCases = await _testCaseService.GetAllTestCasesAsync(moduleId);
        return Ok(testCases);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TestCaseDetailResponse>> GetById(string moduleId, string id)
    {
        var testCase = await _testCaseService.GetTestCaseByIdAsync(moduleId, id);
        return testCase != null ? Ok(testCase) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<IdResponse>> Create(string moduleId, [FromBody] CreateTestCaseRequest request)
    {
        request.ModuleId = moduleId;
        var result = await _testCaseService.CreateTestCaseAsync(request);
        return CreatedAtAction(nameof(GetById), new { moduleId, id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string moduleId, string id, [FromBody] UpdateTestCaseRequest request)
    {
        var success = await _testCaseService.UpdateTestCaseAsync(moduleId, id, request);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string moduleId, string id)
    {
        try
        {
            var success = await _testCaseService.DeleteTestCaseAsync(moduleId, id);
            return success ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting test case {TestCaseId} in module {ModuleId}", id, moduleId);

            return StatusCode(500, new
            {
                success = false,
                message = ex.Message,
                detailed = ex.InnerException?.Message,
                stackTrace = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                    ? ex.StackTrace
                    : null
            });
        }
    }

    [HttpGet("{testCaseId}/attributes")]
    public async Task<ActionResult<IEnumerable<TestCaseAttributeResponse>>> GetAttributes(string moduleId, string testCaseId)
    {
        var attributes = await _testCaseService.GetTestCaseAttributesAsync(moduleId, testCaseId);
        return Ok(attributes);
    }

    [HttpPut("{testCaseId}/attributes")]
    public async Task<IActionResult> UpdateAttributes(string moduleId, string testCaseId,
        [FromBody] IEnumerable<TestCaseAttributeRequest> attributes)
    {
        var success = await _testCaseService.UpdateTestCaseAttributesAsync(moduleId, testCaseId, attributes);
        return success ? NoContent() : NotFound();
    }
}
