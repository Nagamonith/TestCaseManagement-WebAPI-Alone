using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestCaseManagementService.Models.DTOs.TestCases;
using TestCaseManagementService.Models.DTOs.TestSuites;
using TestCaseManagementService.Models.Entities;
using TestCaseManagement.Data;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

using TestCaseManagementService.Models.DTOs.Uploads;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/testsuites/{testSuiteId}/testcases")]
public class TestSuiteTestCasesController : ControllerBase
{
    private readonly ITestSuiteTestCaseService _service;
    private readonly IGenericRepository<TestSuiteTestCase> _repository;
    private readonly ILogger<TestSuiteTestCasesController> _logger;
    private readonly IMapper _mapper;  // Add this
    private readonly AppDbContext _dbContext;

    public TestSuiteTestCasesController(
        ITestSuiteTestCaseService service,
        IGenericRepository<TestSuiteTestCase> repository,
        ILogger<TestSuiteTestCasesController> logger, IMapper mapper, AppDbContext dbContext)
    {
        _service = service;
        _repository = repository;
        _logger = logger;
        _mapper = mapper;  // Add this
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<TestSuiteWithCasesResponse>> GetAll(string testSuiteId)
    {
        try
        {
            _logger.LogInformation("Getting test cases for suite {TestSuiteId}", testSuiteId);

            if (string.IsNullOrWhiteSpace(testSuiteId))
            {
                _logger.LogWarning("Empty test suite ID provided");
                return BadRequest("Test suite ID is required");
            }

            var result = await _service.GetAllTestCasesAsync(testSuiteId);
            _logger.LogInformation("Successfully retrieved {Count} test cases for suite {TestSuiteId}",
                result.TestCases?.Count ?? 0, testSuiteId);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Test suite not found: {TestSuiteId}", testSuiteId);
            return NotFound($"Test suite with ID {testSuiteId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving test cases for suite {TestSuiteId}. Error: {ErrorMessage}",
                testSuiteId, ex.Message);
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
            {
                _logger.LogWarning("Empty test suite ID provided");
                return BadRequest("Test suite ID is required");
            }

            if (request == null || request.TestCaseIds == null || !request.TestCaseIds.Any())
            {
                _logger.LogWarning("No test case IDs provided in request");
                return BadRequest("At least one test case ID is required");
            }

            await _service.AssignTestCasesAsync(testSuiteId, request);

            _logger.LogInformation("Successfully assigned {Count} test cases to suite {TestSuiteId}",
                request.TestCaseIds.Count, testSuiteId);
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
                _logger.LogWarning("Empty test suite ID provided");
                return BadRequest("Test suite ID is required");
            }

            if (string.IsNullOrWhiteSpace(testCaseId))
            {
                _logger.LogWarning("Empty test case ID provided");
                return BadRequest("Test case ID is required");
            }

            var success = await _service.RemoveTestCaseAsync(testSuiteId, testCaseId);

            if (!success)
            {
                _logger.LogWarning("Test case assignment not found: Suite={TestSuiteId}, TestCase={TestCaseId}",
                    testSuiteId, testCaseId);
                return NotFound("Test case assignment not found");
            }

            _logger.LogInformation("Successfully removed test case {TestCaseId} from suite {TestSuiteId}",
                testCaseId, testSuiteId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation during test case removal: Suite={TestSuiteId}, TestCase={TestCaseId}",
                testSuiteId, testCaseId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing test case {TestCaseId} from suite {TestSuiteId}",
                testCaseId, testSuiteId);
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
            {
                _logger.LogWarning("Empty test suite ID provided");
                return BadRequest("Test suite ID is required");
            }

            int count = await _service.RemoveAllAssignmentsAsync(testSuiteId);

            _logger.LogInformation("Successfully removed {Count} test cases from suite {TestSuiteId}",
                count, testSuiteId);
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

    [HttpGet("{testCaseId}/execution")]
    public async Task<ActionResult<TestSuiteTestCaseResponse>> GetExecutionDetails(string testSuiteId, string testCaseId)
    {
        try
        {
            _logger.LogInformation("Getting execution details for test case {TestCaseId} in suite {TestSuiteId}",
                testCaseId, testSuiteId);

            if (string.IsNullOrWhiteSpace(testSuiteId))
            {
                _logger.LogWarning("Empty test suite ID provided");
                return BadRequest("Test suite ID is required");
            }

            if (string.IsNullOrWhiteSpace(testCaseId))
            {
                _logger.LogWarning("Empty test case ID provided");
                return BadRequest("Test case ID is required");
            }

            // Get the TestSuiteTestCase entity
            var testSuiteTestCase = await _dbContext.TestSuiteTestCases
                .Include(t => t.TestCase)
                .Include(t => t.Uploads)
                .FirstOrDefaultAsync(t =>
                    t.TestSuiteId == testSuiteId &&
                    t.TestCaseId == testCaseId);

            if (testSuiteTestCase == null)
            {
                _logger.LogWarning("Test case assignment not found: Suite={TestSuiteId}, TestCase={TestCaseId}",
                    testSuiteId, testCaseId);
                return NotFound("Test case assignment not found");
            }

            // Map to response DTO
            var response = new TestSuiteTestCaseResponse
            {
                Id = testSuiteTestCase.Id,
                TestSuiteId = testSuiteTestCase.TestSuiteId,
                TestCase = _mapper.Map<TestCaseResponse>(testSuiteTestCase.TestCase),
                ExecutionDetails = new ExecutionDetailsResponse
                {
                    TestSuiteTestCaseId = testSuiteTestCase.Id,
                    Result = testSuiteTestCase.Result,
                    Actual = testSuiteTestCase.Actual,
                    Remarks = testSuiteTestCase.Remarks,
                    AddedAt = testSuiteTestCase.AddedAt,
                    UpdatedAt = testSuiteTestCase.UpdatedAt,
                    Uploads = _mapper.Map<List<UploadResponse>>(testSuiteTestCase.Uploads)
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting execution details for test case {TestCaseId} in suite {TestSuiteId}",
                testCaseId, testSuiteId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{testCaseId}/execution")]
    public async Task<ActionResult> UpdateExecutionDetails(string testSuiteId, string testCaseId,
        [FromBody] UpdateExecutionDetailsRequest request)
    {
        try
        {
            _logger.LogInformation("Updating execution details for test case {TestCaseId} in suite {TestSuiteId}",
                testCaseId, testSuiteId);

            if (string.IsNullOrWhiteSpace(testSuiteId))
            {
                _logger.LogWarning("Empty test suite ID provided");
                return BadRequest("Test suite ID is required");
            }

            if (string.IsNullOrWhiteSpace(testCaseId))
            {
                _logger.LogWarning("Empty test case ID provided");
                return BadRequest("Test case ID is required");
            }

            if (request == null)
            {
                _logger.LogWarning("Empty request body provided");
                return BadRequest("Request body is required");
            }

            var testSuiteTestCase = await _repository.FindAsync(t =>
                t.TestSuiteId == testSuiteId && t.TestCaseId == testCaseId);
            var testSuiteTestCaseItem = testSuiteTestCase.FirstOrDefault();

            if (testSuiteTestCaseItem == null)
            {
                _logger.LogWarning("Test case assignment not found: Suite={TestSuiteId}, TestCase={TestCaseId}",
                    testSuiteId, testCaseId);
                return NotFound("Test case assignment not found");
            }

            await _service.UpdateExecutionDetailsAsync(testSuiteTestCaseItem.Id, request);

            _logger.LogInformation("Successfully updated execution details for test case {TestCaseId} in suite {TestSuiteId}",
                testCaseId, testSuiteId);
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
    public async Task<ActionResult> AddExecutionUpload(string testSuiteId, string testCaseId,
    [FromBody] AddExecutionUploadRequest request)
    {
        try
        {
            _logger.LogInformation("Adding upload for test case {TestCaseId} in suite {TestSuiteId}",
                testCaseId, testSuiteId);

            if (string.IsNullOrWhiteSpace(testSuiteId))
            {
                _logger.LogWarning("Empty test suite ID provided");
                return BadRequest("Test suite ID is required");
            }

            if (string.IsNullOrWhiteSpace(testCaseId))
            {
                _logger.LogWarning("Empty test case ID provided");
                return BadRequest("Test case ID is required");
            }

            if (request == null)
            {
                _logger.LogWarning("Empty request body provided");
                return BadRequest("Request body is required");
            }

            var testSuiteTestCase = await _repository.FindAsync(t =>
                t.TestSuiteId == testSuiteId && t.TestCaseId == testCaseId);
            var testSuiteTestCaseItem = testSuiteTestCase.FirstOrDefault();

            if (testSuiteTestCaseItem == null)
            {
                _logger.LogWarning("Test case assignment not found: Suite={TestSuiteId}, TestCase={TestCaseId}",
                    testSuiteId, testCaseId);
                return NotFound("Test case assignment not found");
            }

            var uploadId = await _service.AddExecutionUploadAsync(testSuiteTestCaseItem.Id, request);

            _logger.LogInformation("Successfully added upload {UploadId} for test case {TestCaseId} in suite {TestSuiteId}",
                uploadId, testCaseId, testSuiteId);

            return CreatedAtAction(
                nameof(GetExecutionDetails),
                new { testSuiteId, testCaseId },
                new { UploadId = uploadId });
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
            {
                _logger.LogWarning("Empty upload ID provided");
                return BadRequest("Upload ID is required");
            }

            await _service.RemoveExecutionUploadAsync(uploadId);

            _logger.LogInformation("Successfully removed execution upload {UploadId}", uploadId);
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