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
            {
                return BadRequest("Test suite ID is required");
            }

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

            // Validation
            if (string.IsNullOrWhiteSpace(testSuiteId))
            {
                return BadRequest("Test suite ID is required");
            }

            if (request == null)
            {
                return BadRequest("Request body is required");
            }

            if (request.TestCaseIds == null || !request.TestCaseIds.Any())
            {
                return BadRequest("At least one test case ID is required");
            }

            // Log the request details
            _logger.LogDebug("Assigning {Count} test cases to suite {TestSuiteId}: {TestCaseIds}",
                request.TestCaseIds.Count, testSuiteId, string.Join(", ", request.TestCaseIds));

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
            {
                return BadRequest("Test suite ID is required");
            }

            if (string.IsNullOrWhiteSpace(testCaseId))
            {
                return BadRequest("Test case ID is required");
            }

            var success = await _service.RemoveTestCaseAsync(testSuiteId, testCaseId);

            if (success)
            {
                _logger.LogInformation("Successfully removed test case {TestCaseId} from suite {TestSuiteId}", testCaseId, testSuiteId);
                return NoContent();
            }
            else
            {
                _logger.LogWarning("Test case assignment not found: Suite={TestSuiteId}, TestCase={TestCaseId}", testSuiteId, testCaseId);
                return NotFound($"Test case assignment not found");
            }
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
}