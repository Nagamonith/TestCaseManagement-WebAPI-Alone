using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestCaseManagement.Api.Models.DTOs.TestCases;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations;

public class TestCaseAttributeService : ITestCaseAttributeService
{
    private readonly IGenericRepository<TestCaseAttribute> _attributeRepository;
    private readonly IGenericRepository<TestCase> _testCaseRepository;
    private readonly IGenericRepository<ModuleAttribute> _moduleAttributeRepository;
    private readonly IMapper _mapper;

    public TestCaseAttributeService(
        IGenericRepository<TestCaseAttribute> attributeRepository,
        IGenericRepository<TestCase> testCaseRepository,
        IGenericRepository<ModuleAttribute> moduleAttributeRepository,
        IMapper mapper)
    {
        _attributeRepository = attributeRepository;
        _testCaseRepository = testCaseRepository;
        _moduleAttributeRepository = moduleAttributeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TestCaseAttributeResponse>> GetAllAttributesAsync(string testCaseId)
    {
        var attributes = await _attributeRepository.FindAsync(
            a => a.TestCaseId == testCaseId,
            include: query => query.Include(a => a.ModuleAttribute));

        return attributes.Select(a => new TestCaseAttributeResponse
        {
            Key = a.ModuleAttribute.Key,
            Value = a.Value,
            Name = a.ModuleAttribute.Name,
            Type = a.ModuleAttribute.Type,
            IsRequired = a.ModuleAttribute.IsRequired
        });
    }

    public async Task AddAttributeAsync(string testCaseId, TestCaseAttributeRequest request)
    {
        var testCaseExists = await _testCaseRepository.GetByIdAsync(testCaseId) != null;
        if (!testCaseExists) throw new KeyNotFoundException("Test case not found");

        var moduleAttribute = (await _moduleAttributeRepository
            .FindAsync(ma => ma.Key.ToLower() == request.Key.ToLower()))
            .FirstOrDefault();

        if (moduleAttribute == null)
            throw new KeyNotFoundException($"Module attribute with key '{request.Key}' not found");

        var attribute = new TestCaseAttribute
        {
            TestCaseId = testCaseId,
            ModuleAttributeId = moduleAttribute.Id,
            Value = request.Value
        };

        await _attributeRepository.AddAsync(attribute);
        await _attributeRepository.SaveChangesAsync();
    }

    public async Task<bool> UpdateAttributeAsync(string testCaseId, string key, TestCaseAttributeRequest request)
    {
        var attribute = (await _attributeRepository.FindAsync(
            a => a.TestCaseId == testCaseId,
            include: query => query.Include(a => a.ModuleAttribute)))
            .FirstOrDefault(a => string.Equals(a.ModuleAttribute.Key, key, StringComparison.OrdinalIgnoreCase));

        if (attribute == null) return false;

        if (!string.Equals(key, request.Key, StringComparison.OrdinalIgnoreCase))
        {
            var newModuleAttribute = (await _moduleAttributeRepository
                .FindAsync(ma => ma.Key.ToLower() == request.Key.ToLower()))
                .FirstOrDefault();

            if (newModuleAttribute == null)
                throw new KeyNotFoundException($"Module attribute with key '{request.Key}' not found");

            attribute.ModuleAttributeId = newModuleAttribute.Id;
        }

        attribute.Value = request.Value;
        _attributeRepository.Update(attribute);
        await _attributeRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAttributeAsync(string testCaseId, string key)
    {
        var attribute = (await _attributeRepository.FindAsync(
            a => a.TestCaseId == testCaseId,
            include: query => query.Include(a => a.ModuleAttribute)))
            .FirstOrDefault(a => string.Equals(a.ModuleAttribute.Key, key, StringComparison.OrdinalIgnoreCase));

        if (attribute == null) return false;

        // Clear the value instead of deleting the record
        attribute.Value = string.Empty;
        _attributeRepository.Update(attribute);
        await _attributeRepository.SaveChangesAsync();

        return true;
    }
}
