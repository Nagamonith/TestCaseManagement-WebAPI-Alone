using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.TestSuites;
using TestCaseManagement.Services.Implementations;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/products/{productId}/testsuites")]
public class TestSuitesController : ControllerBase
{
    private readonly ITestSuiteService _testSuiteService;
    private readonly ILogger<TestSuiteService> _logger;

    public TestSuitesController(ITestSuiteService testSuiteService, ILogger<TestSuiteService> logger)
    {
        _testSuiteService = testSuiteService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestSuiteResponse>>> GetAll(string productId)
    {
        var testSuites = await _testSuiteService.GetAllTestSuitesAsync(productId);
        return Ok(testSuites);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TestSuiteResponse>> GetById(string productId, string id)
    {
        var testSuite = await _testSuiteService.GetTestSuiteByIdAsync(productId, id);
        return testSuite != null ? Ok(testSuite) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<IdResponse>> Create(string productId, [FromBody] CreateTestSuiteRequest request)
    {
        var result = await _testSuiteService.CreateTestSuiteAsync(productId, request);
        return CreatedAtAction(nameof(GetById), new { productId, id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string productId, string id, [FromBody] CreateTestSuiteRequest request)
    {
        var success = await _testSuiteService.UpdateTestSuiteAsync(productId, id, request);
        return success ? NoContent() : NotFound();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
    string productId,
    string id,
    [FromQuery] bool forceDelete = false)
    {
        try
        {
            _logger.LogInformation("Attempting to delete test suite {TestSuiteId} for product {ProductId}", id, productId);

            var success = await _testSuiteService.DeleteTestSuiteAsync(productId, id, forceDelete);

            if (!success)
            {
                _logger.LogWarning("Test suite not found: {TestSuiteId}", id);
                return NotFound(new ProblemDetails
                {
                    Title = "Test Suite Not Found",
                    Detail = $"Test suite with ID {id} was not found",
                    Status = 404
                });
            }

            _logger.LogInformation("Successfully deleted test suite {TestSuiteId}", id);
            return NoContent();
        }
        catch (BadHttpRequestException ex)
        {
            _logger.LogWarning(ex, "Conflict deleting test suite {TestSuiteId}", id);
            return Conflict(new ProblemDetails
            {
                Title = "Cannot Delete Test Suite",
                Detail = ex.Message,
                Status = 409
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting test suite {TestSuiteId}", id);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = ex.Message,
                Status = 500
            });
        }
    }
}