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
        // Update test case info and steps
        var testCaseUpdated = await _testCaseService.UpdateTestCaseAsync(moduleId, id, request);
        if (!testCaseUpdated)
            return NotFound();

        // Update attributes if provided
        if (request.Attributes != null)
        {
            var attributesUpdated = await _testCaseService.UpdateTestCaseAttributesAsync(moduleId, id, request.Attributes);
            if (!attributesUpdated)
                return NotFound();
        }

        return NoContent();
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

    // Merged Attribute endpoints from TestCaseAttributesController

    // Create a single attribute
    [HttpPost("{testCaseId}/attributes")]
    public async Task<IActionResult> CreateAttribute(string moduleId, string testCaseId, [FromBody] TestCaseAttributeRequest request)
    {
        try
        {
            await _testCaseService.AddTestCaseAttributeAsync(moduleId, testCaseId, request);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // Update a single attribute by key
    [HttpPut("{testCaseId}/attributes/{key}")]
    public async Task<IActionResult> UpdateAttribute(string moduleId, string testCaseId, string key, [FromBody] TestCaseAttributeRequest request)
    {
        var success = await _testCaseService.UpdateTestCaseAttributeAsync(moduleId, testCaseId, key, request);
        return success ? NoContent() : NotFound();
    }

    // Delete (clear) a single attribute by key
    [HttpDelete("{testCaseId}/attributes/{key}")]
    public async Task<IActionResult> DeleteAttribute(string moduleId, string testCaseId, string key)
    {
        var success = await _testCaseService.DeleteTestCaseAttributeAsync(moduleId, testCaseId, key);
        return success ? NoContent() : NotFound();
    }
}
