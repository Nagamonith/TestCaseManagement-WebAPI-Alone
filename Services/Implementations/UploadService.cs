using Microsoft.EntityFrameworkCore;
using TestCaseManagement.Api.Models.DTOs.Uploads;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Data;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations
{
    public class UploadService : IUploadService
    {
        private readonly AppDbContext _context;
        private readonly string _uploadFolderPath;

        public UploadService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            // Use WebRootPath if available, otherwise fallback to ContentRootPath
            string rootPath = env.WebRootPath ?? env.ContentRootPath;
            _uploadFolderPath = Path.Combine(rootPath, "uploads");

            if (!Directory.Exists(_uploadFolderPath))
                Directory.CreateDirectory(_uploadFolderPath);
        }


        public async Task<UploadResponse> UploadFileAsync(UploadFileRequest request)
        {
            if (request?.File == null || request.File.Length == 0)
                throw new ArgumentException("File is empty");

            string fileId = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(request.File.FileName);
            string fileName = $"{fileId}{extension}";
            string filePath = Path.Combine(_uploadFolderPath, fileName);

            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            var upload = new Upload
            {
                Id = fileId,
                FileName = request.File.FileName,
                FilePath = filePath,
                FileType = request.File.ContentType ?? "",
                FileSize = request.File.Length,
                UploadedAt = DateTime.UtcNow,
                UploadedBy = request.UploadedBy ?? "system",
                TestCaseId = request.TestCaseId
            };

            _context.Set<Upload>().Add(upload);
            await _context.SaveChangesAsync();

            return new UploadResponse
            {
                Id = upload.Id,
                FileName = upload.FileName,
                FileType = upload.FileType,
                FileSize = upload.FileSize,
                UploadedAt = upload.UploadedAt,
                UploadedBy = upload.UploadedBy,
                TestCaseId = upload.TestCaseId
            };
        }

        public async Task<UploadResponse> UploadFileFromBase64Async(UploadBase64FileRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrEmpty(request.Base64File))
                throw new ArgumentException("Base64 file is empty");

            if (string.IsNullOrEmpty(request.FileName))
                throw new ArgumentException("FileName cannot be null or empty");

            var fileBytes = Convert.FromBase64String(request.Base64File);
            string fileId = Guid.NewGuid().ToString();
            string safeFileName = $"{fileId}_{request.FileName}";
            string filePath = Path.Combine(_uploadFolderPath, safeFileName);

            await File.WriteAllBytesAsync(filePath, fileBytes);

            var upload = new Upload
            {
                Id = fileId,
                FileName = request.FileName,
                FilePath = filePath,
                FileType = request.ContentType ?? "",
                FileSize = fileBytes.Length,
                UploadedAt = DateTime.UtcNow,
                UploadedBy = request.UploadedBy ?? "system",
                TestCaseId = request.TestCaseId
            };

            _context.Set<Upload>().Add(upload);
            await _context.SaveChangesAsync();

            return new UploadResponse
            {
                Id = upload.Id,
                FileName = upload.FileName,
                FileType = upload.FileType,
                FileSize = upload.FileSize,
                UploadedAt = upload.UploadedAt,
                UploadedBy = upload.UploadedBy,
                TestCaseId = upload.TestCaseId
            };
        }

        public async Task<(Stream fileStream, string fileName, string contentType)> DownloadFileAsync(string id)
        {
            var upload = await _context.Set<Upload>().FirstOrDefaultAsync(u => u.Id == id);
            if (upload == null)
                throw new FileNotFoundException("File not found");

            if (!File.Exists(upload.FilePath))
                throw new FileNotFoundException("File not found on disk");

            var stream = new FileStream(upload.FilePath, FileMode.Open, FileAccess.Read);
            return (stream, upload.FileName, upload.FileType);
        }

        public async Task<bool> DeleteFileAsync(string id)
        {
            var upload = await _context.Set<Upload>().FirstOrDefaultAsync(u => u.Id == id);
            if (upload == null) return false;

            if (File.Exists(upload.FilePath))
                File.Delete(upload.FilePath);

            _context.Set<Upload>().Remove(upload);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
