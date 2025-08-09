using Microsoft.AspNetCore.Http;
using System.IO;

namespace TestCaseManagement.Api.Utilities;

public class FileUploader
{
    private readonly string _uploadPath;

    public FileUploader(IWebHostEnvironment env)
    {
        _uploadPath = Path.Combine(env.WebRootPath, "uploads");
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<(string FileName, string FilePath, string FileType, long FileSize)> UploadFileAsync(IFormFile file)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(_uploadPath, uniqueFileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return (file.FileName, filePath, file.ContentType, file.Length);
    }

    public Stream GetFileStream(string filePath)
    {
        return new FileStream(filePath, FileMode.Open, FileAccess.Read);
    }

    public void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}