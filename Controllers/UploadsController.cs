using Microsoft.AspNetCore.Mvc;
using TestCaseManagement.Api.Models.DTOs.Uploads;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/uploads")]
public class UploadsController : ControllerBase
{
    private readonly IUploadService _uploadService;

    public UploadsController(IUploadService uploadService)
    {
        _uploadService = uploadService;
    }

    [HttpPost]
    public async Task<ActionResult<UploadResponse>> Upload([FromForm] UploadFileRequest request)
    {
        var result = await _uploadService.UploadFileAsync(request);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Download(string id)
    {
        var (stream, contentType, fileName) = await _uploadService.DownloadFileAsync(id);
        return File(stream, contentType, fileName);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var success = await _uploadService.DeleteFileAsync(id);
        return success ? NoContent() : NotFound();
    }
}