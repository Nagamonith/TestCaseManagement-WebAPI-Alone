using Microsoft.AspNetCore.Mvc;
using TestCaseManagement.Api.Models.DTOs.TestRuns;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Api.Controllers
{
    [ApiController]
    [Route("api/testruns/{testRunId}/testsuites")]
    public class TestRunTestSuitesController : ControllerBase
    {
        private readonly ITestRunTestSuiteService _service;

        public TestRunTestSuitesController(ITestRunTestSuiteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestRunTestSuiteResponse>>> GetAll(string testRunId)
        {
            var result = await _service.GetAllTestSuitesAsync(testRunId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> Assign(string testRunId, [FromBody] AssignTestSuitesRequest request)
        {
            await _service.AssignTestSuitesAsync(testRunId, request);
            return NoContent();
        }

        [HttpPut("{testSuiteId}")]
        public async Task<ActionResult> Update(
            string testRunId,
            string testSuiteId,
            [FromBody] UpdateTestSuiteRequest request)
        {
            var updated = await _service.UpdateTestSuiteAsync(testRunId, testSuiteId, request);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{testSuiteId}")]
        public async Task<ActionResult> Remove(string testRunId, string testSuiteId)
        {
            var removed = await _service.RemoveTestSuiteAsync(testRunId, testSuiteId);
            return removed ? NoContent() : NotFound();
        }
    }
}
