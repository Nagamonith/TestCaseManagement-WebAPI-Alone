using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.Modules;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations
{
    public class ModuleAttributeService : IModuleAttributeService
    {
        private readonly IGenericRepository<ModuleAttribute> _attributeRepository;
        private readonly IGenericRepository<Module> _moduleRepository;
        private readonly IGenericRepository<TestCase> _testCaseRepository;
        private readonly IGenericRepository<TestCaseAttribute> _testCaseAttributeRepository;
        private readonly IMapper _mapper;

        public ModuleAttributeService(
            IGenericRepository<ModuleAttribute> attributeRepository,
            IGenericRepository<Module> moduleRepository,
            IGenericRepository<TestCase> testCaseRepository,
            IGenericRepository<TestCaseAttribute> testCaseAttributeRepository,
            IMapper mapper)
        {
            _attributeRepository = attributeRepository;
            _moduleRepository = moduleRepository;
            _testCaseRepository = testCaseRepository;
            _testCaseAttributeRepository = testCaseAttributeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ModuleAttributeRequest>> GetAllAttributesAsync(string moduleId)
        {
            var attributes = await _attributeRepository.FindAsync(a => a.ModuleId == moduleId);
            return _mapper.Map<IEnumerable<ModuleAttributeRequest>>(attributes);
        }

        public async Task<IdResponse> CreateAttributeAsync(string moduleId, ModuleAttributeRequest request)
        {
            var moduleExists = await _moduleRepository.GetByIdAsync(moduleId) != null;
            if (!moduleExists) throw new KeyNotFoundException("Module not found");

            var attribute = _mapper.Map<ModuleAttribute>(request);
            attribute.ModuleId = moduleId;

            await _attributeRepository.AddAsync(attribute);
            await _attributeRepository.SaveChangesAsync();  // Commit new attribute

            // Now create TestCaseAttributes with empty value for all testcases in this module
            var testCases = await _testCaseRepository.FindAsync(tc => tc.ModuleId == moduleId);

            var testCaseAttributes = testCases.Select(tc => new TestCaseAttribute
            {
                TestCaseId = tc.Id,
                ModuleAttributeId = attribute.Id,
                Value = string.Empty
            });

            foreach (var tca in testCaseAttributes)
            {
                await _testCaseAttributeRepository.AddAsync(tca);
            }
            await _testCaseAttributeRepository.SaveChangesAsync();

            return new IdResponse { Id = attribute.Id };
        }

        public async Task<bool> UpdateAttributeAsync(string moduleId, string attributeId, ModuleAttributeRequest request)
        {
            var attribute = (await _attributeRepository.FindAsync(a => a.ModuleId == moduleId && a.Id == attributeId)).FirstOrDefault();
            if (attribute == null) return false;

            attribute.Name = request.Name;
            attribute.Key = request.Key;
            attribute.Type = request.Type;
            attribute.IsRequired = request.IsRequired;
            attribute.Options = request.Options;

            _attributeRepository.Update(attribute);
            await _attributeRepository.SaveChangesAsync();  // Commit update

            return true;
        }

        public async Task<bool> DeleteAttributeAsync(string moduleId, string id)
        {
            var attribute = (await _attributeRepository.FindAsync(a => a.ModuleId == moduleId && a.Id == id)).FirstOrDefault();
            if (attribute == null) return false;

            // Delete related TestCaseAttributes first
            var relatedTestCaseAttrs = await _testCaseAttributeRepository.FindAsync(tca => tca.ModuleAttributeId == attribute.Id);
            foreach (var tca in relatedTestCaseAttrs)
            {
                _testCaseAttributeRepository.Remove(tca);
            }
            await _testCaseAttributeRepository.SaveChangesAsync();

            // Now delete the ModuleAttribute
            _attributeRepository.Remove(attribute);
            await _attributeRepository.SaveChangesAsync();  // Commit deletion

            return true;
        }
    }
}
