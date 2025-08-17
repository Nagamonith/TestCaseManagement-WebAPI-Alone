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

            // Configure Product cascades
            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductVersions)
                .WithOne(pv => pv.Product)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Modules)
                .WithOne(m => m.Product)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.TestSuites)
                .WithOne(ts => ts.Product)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.TestRuns)
                .WithOne(tr => tr.Product)
                .OnDelete(DeleteBehavior.ClientCascade);

            // Configure ProductVersion - set null when deleted
            modelBuilder.Entity<ProductVersion>()
                .HasMany(pv => pv.Modules)
                .WithOne(m => m.ProductVersion)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ProductVersion>()
                .HasMany(pv => pv.TestCases)
                .WithOne(tc => tc.ProductVersion)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ProductVersion>()
                .HasMany(pv => pv.TestSuiteTestCases)
                .WithOne(tstc => tstc.ProductVersion)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Module cascades
            modelBuilder.Entity<TestCaseManagement.Api.Models.Entities.Module>()
                .HasMany(m => m.TestCases)
                .WithOne(tc => tc.Module)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestCaseManagement.Api.Models.Entities.Module>()
                .HasMany(m => m.ModuleAttributes)
                .WithOne(ma => ma.Module)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure TestCase cascades
            modelBuilder.Entity<TestCase>()
                .HasMany(tc => tc.ManualTestCaseSteps)
                .WithOne(mts => mts.TestCase)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestCase>()
                .HasMany(tc => tc.TestCaseAttributes)
                .WithOne(tca => tca.TestCase)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestCase>()
                .HasMany(tc => tc.TestSuiteTestCases)
                .WithOne(tstc => tstc.TestCase)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestCase>()
                .HasMany(tc => tc.TestRunResults)
                .WithOne(trr => trr.TestCase)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure TestSuite cascades
            modelBuilder.Entity<TestSuite>()
                .HasMany(ts => ts.TestSuiteTestCases)
                .WithOne(tstc => tstc.TestSuite)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestSuite>()
                .HasMany(ts => ts.TestRunTestSuites)
                .WithOne(trts => trts.TestSuite)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure TestRun cascades
            modelBuilder.Entity<TestRun>()
                .HasMany(tr => tr.TestRunTestSuites)
                .WithOne(trts => trts.TestRun)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestRun>()
                .HasMany(tr => tr.TestRunResults)
                .WithOne(trr => trr.TestRun)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure TestSuiteTestCase cascades
            modelBuilder.Entity<TestSuiteTestCase>()
                .HasMany(tstc => tstc.Uploads)
                .WithOne(u => u.TestSuiteTestCase)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Upload relationships (keep existing)
            modelBuilder.Entity<Upload>()
                .HasOne(u => u.TestCase)
                .WithMany(tc => tc.Uploads)
                .HasForeignKey(u => u.TestCaseId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Upload>()
                .HasOne(u => u.TestSuiteTestCase)
                .WithMany(tstc => tstc.Uploads)
                .HasForeignKey(u => u.TestSuiteTestCaseId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}