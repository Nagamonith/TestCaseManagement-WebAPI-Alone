using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestCaseManagement.Api.Models.DTOs.TestSuites;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/testsuites/{testSuiteId}/testcases")]
public class TestSuiteTestCasesController : ControllerBase
{
    private readonly ITestSuiteTestCaseService _service;
    private readonly IGenericRepository<TestSuiteTestCase> _repository;
    private readonly ILogger<TestSuiteTestCasesController> _logger;

    public TestSuiteTestCasesController(
        ITestSuiteTestCaseService service,
        IGenericRepository<TestSuiteTestCase> repository,
        ILogger<TestSuiteTestCasesController> logger)
    {
        _service = service;
        _repository = repository;
        _logger = logger;
    }

    // ===== Existing endpoints =====

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

    // ===== Execution endpoints =====

    [HttpGet("{testCaseId}/execution")]
    public async Task<ActionResult<TestSuiteTestCaseResponse>> GetExecutionDetails(string testSuiteId, string testCaseId)
    {
        try
        {
            _logger.LogInformation("Getting execution details for test case {TestCaseId} in suite {TestSuiteId}",
                testCaseId, testSuiteId);

            if (string.IsNullOrWhiteSpace(testSuiteId))
                return BadRequest("Test suite ID is required");
            if (string.IsNullOrWhiteSpace(testCaseId))
                return BadRequest("Test case ID is required");

            var testSuiteTestCase = await _repository.FindAsync(t =>
                t.TestSuiteId == testSuiteId && t.TestCaseId == testCaseId);
            var testSuiteTestCaseItem = testSuiteTestCase.FirstOrDefault();

            if (testSuiteTestCaseItem == null)
                return NotFound("Test case assignment not found");

            var result = await _service.GetTestCaseExecutionDetailsAsync(testSuiteTestCaseItem.Id);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Execution details not found for test case {TestCaseId} in suite {TestSuiteId}",
                testCaseId, testSuiteId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting execution details for test case {TestCaseId} in suite {TestSuiteId}",
                testCaseId, testSuiteId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{testCaseId}/execution")]
    public async Task<ActionResult> UpdateExecutionDetails(string testSuiteId, string testCaseId, [FromBody] UpdateExecutionDetailsRequest request)
    {
        try
        {
            _logger.LogInformation("Updating execution details for test case {TestCaseId} in suite {TestSuiteId}",
                testCaseId, testSuiteId);

            if (string.IsNullOrWhiteSpace(testSuiteId))
                return BadRequest("Test suite ID is required");
            if (string.IsNullOrWhiteSpace(testCaseId))
                return BadRequest("Test case ID is required");
            if (request == null)
                return BadRequest("Request body is required");

            var testSuiteTestCase = await _repository.FindAsync(t =>
                t.TestSuiteId == testSuiteId && t.TestCaseId == testCaseId);
            var testSuiteTestCaseItem = testSuiteTestCase.FirstOrDefault();

            if (testSuiteTestCaseItem == null)
                return NotFound("Test case assignment not found");

            await _service.UpdateExecutionDetailsAsync(testSuiteTestCaseItem.Id, request);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Test case assignment not found: {TestCaseId} in suite {TestSuiteId}",
                testCaseId, testSuiteId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating execution details for test case {TestCaseId} in suite {TestSuiteId}",
                testCaseId, testSuiteId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{testCaseId}/uploads")]
    public async Task<ActionResult> AddExecutionUpload(string testSuiteId, string testCaseId, [FromBody] AddExecutionUploadRequest request)
    {
        try
        {
            _logger.LogInformation("Adding upload for test case {TestCaseId} in suite {TestSuiteId}",
                testCaseId, testSuiteId);

            if (string.IsNullOrWhiteSpace(testSuiteId))
                return BadRequest("Test suite ID is required");
            if (string.IsNullOrWhiteSpace(testCaseId))
                return BadRequest("Test case ID is required");
            if (request == null)
                return BadRequest("Request body is required");

            var testSuiteTestCase = await _repository.FindAsync(t =>
                t.TestSuiteId == testSuiteId && t.TestCaseId == testCaseId);
            var testSuiteTestCaseItem = testSuiteTestCase.FirstOrDefault();

            if (testSuiteTestCaseItem == null)
                return NotFound("Test case assignment not found");

            await _service.AddExecutionUploadAsync(testSuiteTestCaseItem.Id, request);
            return CreatedAtAction(nameof(GetExecutionDetails), new { testSuiteId, testCaseId }, null);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Test case assignment not found: {TestCaseId} in suite {TestSuiteId}",
                testCaseId, testSuiteId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding upload for test case {TestCaseId} in suite {TestSuiteId}",
                testCaseId, testSuiteId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("uploads/{uploadId}")]
    public async Task<ActionResult> RemoveExecutionUpload(string uploadId)
    {
        try
        {
            _logger.LogInformation("Removing execution upload {UploadId}", uploadId);

            if (string.IsNullOrWhiteSpace(uploadId))
                return BadRequest("Upload ID is required");

            await _service.RemoveExecutionUploadAsync(uploadId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Upload not found: {UploadId}", uploadId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing upload {UploadId}", uploadId);
            return StatusCode(500, "Internal server error");
        }
    }
}