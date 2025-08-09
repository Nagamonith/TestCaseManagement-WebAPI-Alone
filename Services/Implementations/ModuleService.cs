using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.Modules;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Data;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations;

public class ModuleService : IModuleService
{
    private readonly IGenericRepository<Module> _moduleRepository;
    private readonly IGenericRepository<Product> _productRepository;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ModuleService> _logger;

    public ModuleService(
        IGenericRepository<Module> moduleRepository,
        IGenericRepository<Product> productRepository,
        AppDbContext dbContext,
        ILogger<ModuleService> logger)
    {
        _moduleRepository = moduleRepository;
        _productRepository = productRepository;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<ModuleResponse>> GetAllModulesAsync(string productId)
    {
        try
        {
            return await _dbContext.Modules
                .Where(m => m.ProductId == productId)
                .Select(m => new ModuleResponse
                {
                    Id = m.Id,
                    ProductId = m.ProductId,
                    Version = m.Version,
                    Name = m.Name,
                    Description = m.Description,
                    CreatedAt = m.CreatedAt,
                    IsActive = m.IsActive
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting modules for product {ProductId}", productId);
            throw new ApplicationException("Error retrieving modules", ex);
        }
    }

    public async Task<ModuleResponse?> GetModuleByIdAsync(string productId, string id)
    {
        try
        {
            var module = await _dbContext.Modules
                .Where(m => m.ProductId == productId && m.Id == id)
                .Select(m => new ModuleResponse
                {
                    Id = m.Id,
                    ProductId = m.ProductId,
                    Version = m.Version,
                    Name = m.Name,
                    Description = m.Description,
                    CreatedAt = m.CreatedAt,
                    IsActive = m.IsActive
                })
                .FirstOrDefaultAsync();

            return module;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting module {ModuleId} for product {ProductId}", id, productId);
            throw new ApplicationException("Error retrieving module", ex);
        }
    }

    public async Task<IdResponse> CreateModuleAsync(CreateModuleRequest request)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var module = new Module
            {
                ProductId = request.ProductId,
                Version = request.Version.Trim(),
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                IsActive = request.IsActive
                // CreatedAt is automatically set by entity
            };

            await _dbContext.Modules.AddAsync(module);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new IdResponse { Id = module.Id };
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
        {
            await transaction.RollbackAsync();
            _logger.LogError("SQL Error {Number}: {Message}", sqlEx.Number, sqlEx.Message);
            throw new ApplicationException("Database operation failed", ex);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating module");
            throw;
        }
    }

    public async Task<bool> UpdateModuleAsync(string id, CreateModuleRequest request)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var module = await _moduleRepository.GetByIdAsync(id);
            if (module == null) return false;

            module.Name = request.Name;
            module.Version = request.Version;
            module.Description = request.Description;
            module.IsActive = request.IsActive;

            _moduleRepository.Update(module);
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating module {ModuleId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteModuleAsync(string productId, string id)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var module = await _dbContext.Modules
                .FirstOrDefaultAsync(m => m.ProductId == productId && m.Id == id);

            if (module == null) return false;

            _moduleRepository.Remove(module);
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error deleting module {ModuleId} from product {ProductId}", id, productId);
            throw;
        }
    }
}