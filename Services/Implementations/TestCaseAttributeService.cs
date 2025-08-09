using AutoMapper;
using TestCaseManagement.Api.Models.DTOs.TestCases;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations;

public class TestCaseAttributeService : ITestCaseAttributeService
{
    private readonly IGenericRepository<TestCaseAttribute> _attributeRepository;
    private readonly IGenericRepository<TestCase> _testCaseRepository;
    private readonly IMapper _mapper;

    public TestCaseAttributeService(
        IGenericRepository<TestCaseAttribute> attributeRepository,
        IGenericRepository<TestCase> testCaseRepository,
        IMapper mapper)
    {
        _attributeRepository = attributeRepository;
        _testCaseRepository = testCaseRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TestCaseAttributeResponse>> GetAllAttributesAsync(string testCaseId)
    {
        var attributes = await _attributeRepository.FindAsync(a => a.TestCaseId == testCaseId);
        return _mapper.Map<IEnumerable<TestCaseAttributeResponse>>(attributes);
    }

    public async Task AddAttributeAsync(string testCaseId, TestCaseAttributeRequest request)
    {
        var testCaseExists = await _testCaseRepository.GetByIdAsync(testCaseId) != null;
        if (!testCaseExists) throw new KeyNotFoundException("Test case not found");

        var attribute = _mapper.Map<TestCaseAttribute>(request);
        attribute.TestCaseId = testCaseId;

        await _attributeRepository.AddAsync(attribute);
    }

    public async Task<bool> DeleteAttributeAsync(string testCaseId, string key)
    {
        var attribute = (await _attributeRepository.FindAsync(a => a.TestCaseId == testCaseId && a.Key == key)).FirstOrDefault();
        if (attribute == null) return false;

        _attributeRepository.Remove(attribute);
        return true;
    }
}