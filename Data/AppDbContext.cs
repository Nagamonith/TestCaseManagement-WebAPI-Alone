using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Data.Configurations;

namespace TestCaseManagement.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVersion> ProductVersions { get; set; }
    // In AppDbContext.cs
    public DbSet<TestCaseManagement.Api.Models.Entities.Module> Modules { get; set; }
    public DbSet<ModuleAttribute> ModuleAttributes { get; set; }
    public DbSet<TestSuite> TestSuites { get; set; }
    public DbSet<TestCase> TestCases { get; set; }
    public DbSet<ManualTestCaseStep> ManualTestCaseSteps { get; set; }
    public DbSet<TestCaseAttribute> TestCaseAttributes { get; set; }
    public DbSet<Upload> Uploads { get; set; }
    public DbSet<TestSuiteTestCase> TestSuiteTestCases { get; set; }
    public DbSet<TestRun> TestRuns { get; set; }
    public DbSet<TestRunTestSuite> TestRunTestSuites { get; set; }
    public DbSet<TestRunResult> TestRunResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new ProductVersionConfiguration());
        modelBuilder.ApplyConfiguration(new ModuleConfiguration());
        modelBuilder.ApplyConfiguration(new ModuleAttributeConfiguration());
        modelBuilder.ApplyConfiguration(new TestSuiteConfiguration());
        modelBuilder.ApplyConfiguration(new TestCaseConfiguration());
        modelBuilder.ApplyConfiguration(new ManualTestCaseStepConfiguration());
        modelBuilder.ApplyConfiguration(new TestCaseAttributeConfiguration());
        modelBuilder.ApplyConfiguration(new UploadConfiguration());
        modelBuilder.ApplyConfiguration(new TestSuiteTestCaseConfiguration());
        modelBuilder.ApplyConfiguration(new TestRunConfiguration());
        modelBuilder.ApplyConfiguration(new TestRunTestSuiteConfiguration());
        modelBuilder.ApplyConfiguration(new TestRunResultConfiguration());

    }
}