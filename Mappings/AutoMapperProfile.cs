using AutoMapper;
using TestCaseManagement.Api.Models.DTOs.Modules;
using TestCaseManagement.Api.Models.DTOs.Products;
using TestCaseManagement.Api.Models.DTOs.TestCases;
using TestCaseManagement.Api.Models.DTOs.TestRuns;
using TestCaseManagement.Api.Models.DTOs.TestSuites;
using TestCaseManagement.Api.Models.DTOs.Uploads;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Api.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Product mappings
            CreateMap<CreateProductRequest, Product>();
            CreateMap<UpdateProductRequest, Product>();
            CreateMap<Product, ProductResponse>();

            // ProductVersion mappings
            CreateMap<ProductVersionRequest, ProductVersion>();
            CreateMap<ProductVersion, ProductVersionRequest>();

            // Module mappings
            CreateMap<CreateModuleRequest, Module>();
            CreateMap<Module, ModuleResponse>();
            CreateMap<Module, ModuleWithAttributesResponse>();

            // ModuleAttribute mappings
            CreateMap<ModuleAttributeRequest, ModuleAttribute>();
            CreateMap<ModuleAttribute, ModuleAttributeRequest>();

            // TestSuite mappings
            CreateMap<CreateTestSuiteRequest, TestSuite>();
            CreateMap<TestSuite, TestSuiteResponse>();
            CreateMap<TestSuite, TestSuiteWithCasesResponse>();

            // TestCase mappings
            CreateMap<CreateTestCaseRequest, TestCase>();
            CreateMap<UpdateTestCaseRequest, TestCase>();

            // ✅ Updated TestCase → TestCaseResponse mapping (explicitly includes Actual & Remarks)
            CreateMap<TestCase, TestCaseResponse>()
                .ForMember(dest => dest.Actual, opt => opt.MapFrom(src => src.Actual))
                .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks));

            // ✅ Ensure TestCaseDetailResponse also includes them (if needed)
            CreateMap<TestCase, TestCaseDetailResponse>()
                .ForMember(dest => dest.Actual, opt => opt.MapFrom(src => src.Actual))
                .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks));

            // ManualTestCaseStep mappings
            CreateMap<ManualTestCaseStepRequest, ManualTestCaseStep>();
            CreateMap<ManualTestCaseStep, ManualTestCaseStepRequest>();

            // TestCaseAttribute mappings
            CreateMap<TestCaseAttributeRequest, TestCaseAttribute>();
            CreateMap<TestCaseAttribute, TestCaseAttributeResponse>();

            // Upload mappings
            CreateMap<Upload, UploadResponse>();

            // TestRun mappings
            CreateMap<CreateTestRunRequest, TestRun>();
            CreateMap<TestRun, TestRunResponse>();

            // TestRunResult mappings
            CreateMap<TestRunResult, TestRunResultResponse>();
        }
    }
}