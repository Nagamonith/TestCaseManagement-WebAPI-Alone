using Microsoft.EntityFrameworkCore;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data;

public class DbInitializer
{
    private readonly AppDbContext _context;

    public DbInitializer(AppDbContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync()
    {
        await _context.Database.MigrateAsync();

        if (!await _context.Products.AnyAsync())
        {
            var products = new List<Product>
            {
                new() { Id = Guid.NewGuid().ToString(), Name = "Test Management System", Description = "Core product for managing test cases", IsActive = true },
                new() { Id = Guid.NewGuid().ToString(), Name = "Reporting Module", Description = "Advanced reporting features", IsActive = true }
            };

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            var versions = new List<ProductVersion>
            {
                new() { Id = Guid.NewGuid().ToString(), ProductId = products[0].Id, Version = "1.0.0", IsActive = true },
                new() { Id = Guid.NewGuid().ToString(), ProductId = products[0].Id, Version = "2.0.0", IsActive = true }
            };

            await _context.ProductVersions.AddRangeAsync(versions);
            await _context.SaveChangesAsync();
        }
    }
}