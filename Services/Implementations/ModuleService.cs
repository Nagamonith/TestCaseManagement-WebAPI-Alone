using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.Modules;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Data;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations
{
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
                    .Include(m => m.ProductVersion) // Include the related ProductVersion
                    .Where(m => m.ProductId == productId)
                    .Select(m => new ModuleResponse
                    {
                        Id = m.Id,
                        ProductId = m.ProductId,
                        Version = m.ProductVersion != null ? m.ProductVersion.Version : string.Empty,
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
                    .Include(m => m.ProductVersion) // Include the related ProductVersion
                    .Where(m => m.ProductId == productId && m.Id == id)
                    .Select(m => new ModuleResponse
                    {
                        Id = m.Id,
                        ProductId = m.ProductId,
                        Version = m.ProductVersion != null ? m.ProductVersion.Version : string.Empty,
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
                // Ensure product exists
                var product = await _productRepository.GetByIdAsync(request.ProductId);
                if (product == null)
                    throw new KeyNotFoundException("Product not found");

                // Determine ProductVersionId:
                string productVersionId = request.ProductVersionId?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(productVersionId))
                {
                    // Lookup latest active product version for the product
                    var latestVersion = await _dbContext.ProductVersions
                        .Where(v => v.ProductId == request.ProductId && v.IsActive)
                        .OrderByDescending(v => v.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (latestVersion == null)
                        throw new KeyNotFoundException("No product version found for the specified product. Create a product version first.");

                    productVersionId = latestVersion.Id;
                }
                else
                {
                    // Validate provided ProductVersionId belongs to the Product
                    var exists = await _dbContext.ProductVersions
                        .AnyAsync(v => v.Id == productVersionId && v.ProductId == request.ProductId);
                    if (!exists)
                        throw new KeyNotFoundException("Provided ProductVersionId is invalid for the specified product.");
                }

                var module = new Module
                {
                    ProductId = request.ProductId,
                    ProductVersionId = productVersionId,
                    Name = request.Name.Trim(),
                    Description = request.Description?.Trim(),
                    IsActive = request.IsActive
                    // CreatedAt is set by default on the entity
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

                // Optionally validate product id (ensure module belongs to product in request)
                if (!string.Equals(module.ProductId, request.ProductId, StringComparison.OrdinalIgnoreCase))
                {
                    // If request.ProductId differs from module.ProductId, reject to avoid moving module between products
                    throw new InvalidOperationException("ProductId mismatch. Module cannot be moved between products.");
                }

                module.Name = request.Name.Trim();
                module.Description = request.Description?.Trim();
                module.IsActive = request.IsActive;

                // Handle ProductVersionId update:
                var newVersionId = request.ProductVersionId?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(newVersionId))
                {
                    // If no version provided on update, we set it to the latest active version automatically
                    var latestVersion = await _dbContext.ProductVersions
                        .Where(v => v.ProductId == module.ProductId && v.IsActive)
                        .OrderByDescending(v => v.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (latestVersion != null)
                    {
                        module.ProductVersionId = latestVersion.Id;
                    }
                    // if no latestVersion found, keep existing version (or you may decide to throw)
                }
                else
                {
                    // Validate provided ProductVersionId belongs to the product
                    var exists = await _dbContext.ProductVersions
                        .AnyAsync(v => v.Id == newVersionId && v.ProductId == module.ProductId);

                    if (!exists)
                        throw new KeyNotFoundException("Provided ProductVersionId is invalid for this module's product.");

                    module.ProductVersionId = newVersionId;
                }

                _moduleRepository.Update(module);
                await _dbContext.SaveChangesAsync();
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

                _dbContext.Modules.Remove(module);

                await _dbContext.SaveChangesAsync();
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
}
