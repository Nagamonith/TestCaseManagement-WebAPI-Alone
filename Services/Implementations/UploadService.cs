using AutoMapper;
using Microsoft.AspNetCore.Http;
using TestCaseManagement.Api.Models.DTOs.Uploads;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Api.Utilities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations;

public class UploadService : IUploadService
{
    private readonly IGenericRepository<Upload> _uploadRepository;
    private readonly IGenericRepository<TestCase> _testCaseRepository;
    private readonly FileUploader _fileUploader;
    private readonly IMapper _mapper;

    public UploadService(
        IGenericRepository<Upload> uploadRepository,
        IGenericRepository<TestCase> testCaseRepository,
        FileUploader fileUploader,
        IMapper mapper)
    {
        _uploadRepository = uploadRepository;
        _testCaseRepository = testCaseRepository;
        _fileUploader = fileUploader;
        _mapper = mapper;
    }

    public async Task<UploadResponse> UploadFileAsync(UploadFileRequest request)
    {
        if (request.TestCaseId != null)
        {
            var testCaseExists = await _testCaseRepository.GetByIdAsync(request.TestCaseId) != null;
            if (!testCaseExists) throw new KeyNotFoundException("Test case not found");
        }

        var uploadResult = await _fileUploader.UploadFileAsync(request.File);

        var upload = new Upload
        {
            FileName = uploadResult.FileName,
            FilePath = uploadResult.FilePath,
            FileType = uploadResult.FileType,
            FileSize = uploadResult.FileSize,
            UploadedBy = "system", // Replace with actual user from auth context
            TestCaseId = request.TestCaseId
        };

        await _uploadRepository.AddAsync(upload);
        return _mapper.Map<UploadResponse>(upload);
    }

    public async Task<(Stream, string, string)> DownloadFileAsync(string id)
    {
        var upload = await _uploadRepository.GetByIdAsync(id);
        if (upload == null) throw new KeyNotFoundException("File not found");

        var stream = _fileUploader.GetFileStream(upload.FilePath);
        return (stream, upload.FileType, upload.FileName);
    }

    public async Task<bool> DeleteFileAsync(string id)
    {
        var upload = await _uploadRepository.GetByIdAsync(id);
        if (upload == null) return false;

        _fileUploader.DeleteFile(upload.FilePath);
        _uploadRepository.Remove(upload);
        return true;
    }
}