using Microsoft.AspNetCore.Mvc;
using TestCaseManagement.Api.Models.DTOs.TestSuites;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/testsuites/{testSuiteId}/testcases")]
public class TestSuiteTestCasesController : ControllerBase
{
    private readonly ITestSuiteTestCaseService _service;
    private readonly ILogger<TestSuiteTestCasesController> _logger;

    public TestSuiteTestCasesController(
        ITestSuiteTestCaseService service,
        ILogger<TestSuiteTestCasesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<TestSuiteWithCasesResponse>> GetAll(string testSuiteId)
    {
        try
        {
            _logger.LogInformation("Getting test cases for suite {TestSuiteId}", testSuiteId);

            if (string.IsNullOrWhiteSpace(testSuiteId))
                return BadRequest("Test suite ID is required");

            var result = await _service.GetAllTestCasesAsync(testSuiteId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Test suite not found: {TestSuiteId}", testSuiteId);
            return NotFound($"Test suite with ID {testSuiteId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving test cases for suite {TestSuiteId}", testSuiteId);
            return StatusCode(500, "Internal server error occurred while retrieving test cases");
        }
    }

    [HttpPost]
    public async Task<ActionResult> Assign(string testSuiteId, [FromBody] AssignTestCasesRequest request)
    {
        try
        {
            _logger.LogInformation("Assigning test cases to suite {TestSuiteId}", testSuiteId);

            if (string.IsNullOrWhiteSpace(testSuiteId))
                return BadRequest("Test suite ID is required");

            if (request == null || request.TestCaseIds == null || !request.TestCaseIds.Any())
                return BadRequest("At least one test case ID is required");

            await _service.AssignTestCasesAsync(testSuiteId, request);

            _logger.LogInformation("Successfully assigned test cases to suite {TestSuiteId}", testSuiteId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Test suite not found during assignment: {TestSuiteId}", testSuiteId);
            return NotFound($"Test suite with ID {testSuiteId} not found");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation during test case assignment for suite {TestSuiteId}", testSuiteId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning test cases to suite {TestSuiteId}", testSuiteId);
            return StatusCode(500, "Internal server error occurred while assigning test cases");
        }
    }

    [HttpDelete("{testCaseId}")]
    public async Task<ActionResult> Remove(string testSuiteId, string testCaseId)
    {
        try
        {
            _logger.LogInformation("Removing test case {TestCaseId} from suite {TestSuiteId}", testCaseId, testSuiteId);

            if (string.IsNullOrWhiteSpace(testSuiteId))
                return BadRequest("Test suite ID is required");

            if (string.IsNullOrWhiteSpace(testCaseId))
                return BadRequest("Test case ID is required");

            var success = await _service.RemoveTestCaseAsync(testSuiteId, testCaseId);

            return success ? NoContent() : NotFound("Test case assignment not found");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation during test case removal: Suite={TestSuiteId}, TestCase={TestCaseId}", testSuiteId, testCaseId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing test case {TestCaseId} from suite {TestSuiteId}", testCaseId, testSuiteId);
            return StatusCode(500, "Internal server error occurred while removing test case");
        }
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveAll(string testSuiteId)
    {
        try
        {
            _logger.LogInformation("Removing all test case assignments from suite {TestSuiteId}", testSuiteId);

            if (string.IsNullOrWhiteSpace(testSuiteId))
                return BadRequest("Test suite ID is required");

            await _service.RemoveAllAssignmentsAsync(testSuiteId);

            _logger.LogInformation("Successfully removed all test cases from suite {TestSuiteId}", testSuiteId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation during removing all test cases for suite {TestSuiteId}", testSuiteId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing all test cases from suite {TestSuiteId}", testSuiteId);
            return StatusCode(500, "Internal server error occurred while removing all test cases");
        }
    }
}
