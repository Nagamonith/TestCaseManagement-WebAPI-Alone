using TestCaseManagement.Api.Models.DTOs.Uploads;

namespace TestCaseManagement.Services.Interfaces;

public interface IUploadService
{
    Task<UploadResponse> UploadFileAsync(UploadFileRequest request);
    Task<(Stream, string, string)> DownloadFileAsync(string id);
    Task<bool> DeleteFileAsync(string id);
}