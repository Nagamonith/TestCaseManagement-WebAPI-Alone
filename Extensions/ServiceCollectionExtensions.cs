using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TestCaseManagement.Api.Utilities;
using TestCaseManagement.Api.Validators;
using TestCaseManagement.Data;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Implementations;
using TestCaseManagement.Services.Interfaces;
using TestCaseManagementService.Repositories.Implementations;

namespace TestCaseManagement.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    public static IServiceCollection AddAutoMapper(this IServiceCollection services, Assembly assembly)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductVersionService, ProductVersionService>();
        services.AddScoped<IModuleService, ModuleService>();
        services.AddScoped<IModuleAttributeService, ModuleAttributeService>();
        services.AddScoped<ITestCaseService, TestCaseService>();
        services.AddScoped<IManualTestCaseStepService, ManualTestCaseStepService>();
        services.AddScoped<ITestCaseAttributeService, TestCaseAttributeService>();
        services.AddScoped<ITestSuiteService, TestSuiteService>();
        services.AddScoped<ITestSuiteTestCaseService, TestSuiteTestCaseService>();
        services.AddScoped<ITestRunService, TestRunService>();
        services.AddScoped<ITestRunTestSuiteService, TestRunTestSuiteService>();
        services.AddScoped<ITestRunResultService, TestRunResultService>();
        services.AddScoped<IUploadService, UploadService>();

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
        return services;
    }

    public static IServiceCollection AddUtilities(this IServiceCollection services)
    {
        services.AddScoped<FileUploader>();
        services.AddScoped<ExcelExporter>();
        services.AddScoped<PdfGenerator>();
        services.AddScoped<EmailNotifier>();

        return services;
    }
}