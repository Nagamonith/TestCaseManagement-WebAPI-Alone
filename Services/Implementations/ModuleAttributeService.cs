using AutoMapper;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.Modules;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations;

public class ModuleAttributeService : IModuleAttributeService
{
    private readonly IGenericRepository<ModuleAttribute> _attributeRepository;
    private readonly IGenericRepository<Module> _moduleRepository;
    private readonly IMapper _mapper;

    public ModuleAttributeService(
        IGenericRepository<ModuleAttribute> attributeRepository,
        IGenericRepository<Module> moduleRepository,
        IMapper mapper)
    {
        _attributeRepository = attributeRepository;
        _moduleRepository = moduleRepository;
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
        return true;
    }


    public async Task<bool> DeleteAttributeAsync(string moduleId, string id)
    {
        var attribute = (await _attributeRepository.FindAsync(a => a.ModuleId == moduleId && a.Id == id)).FirstOrDefault();
        if (attribute == null) return false;

        _attributeRepository.Remove(attribute);
        return true;
    }
}