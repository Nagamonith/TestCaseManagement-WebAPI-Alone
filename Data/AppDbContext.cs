using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVersion> ProductVersions { get; set; }
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

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
