using Microsoft.AspNetCore.Mvc;
using TestCaseManagement.Api.Models.DTOs.Uploads;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Api.Controllers
{
    [ApiController]
    [Route("api/uploads")]
    public class UploadsController : ControllerBase
    {
        private readonly IUploadService _uploadService;

        public UploadsController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        /// <summary>
        /// Upload a file via multipart/form-data. Optionally link it to a test case by providing TestCaseId.
        /// </summary>
        [HttpPost("file")]
        public async Task<ActionResult<UploadResponse>> Upload([FromForm] UploadFileRequest request)
        {
            try
            {
                var result = await _uploadService.UploadFileAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Upload a file via Base64 string. Optionally link it to a test case by providing TestCaseId.
        /// </summary>
        [HttpPost("base64")]
        public async Task<ActionResult<UploadResponse>> UploadBase64([FromBody] UploadBase64FileRequest request)
        {
            try
            {
                var result = await _uploadService.UploadFileFromBase64Async(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Download a file by its upload ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult> Download(string id)
        {
            try
            {
                var (stream, fileName, contentType) = await _uploadService.DownloadFileAsync(id);
                return File(stream, contentType, fileName);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a file by its upload ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var success = await _uploadService.DeleteFileAsync(id);
            return success ? NoContent() : NotFound(new { message = "File not found" });
        }
    }
}
