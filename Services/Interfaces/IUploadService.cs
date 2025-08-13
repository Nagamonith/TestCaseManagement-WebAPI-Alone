using TestCaseManagement.Api.Models.DTOs.Uploads;

namespace TestCaseManagement.Services.Interfaces
{
    public interface IUploadService
    {
        Task<UploadResponse> UploadFileAsync(UploadFileRequest request);
        Task<(Stream fileStream, string fileName, string contentType)> DownloadFileAsync(string id);
        Task<bool> DeleteFileAsync(string id);

        Task<UploadResponse> UploadFileFromBase64Async(UploadBase64FileRequest request);
    }
}
