using AutoMapper;
    using System.Linq;
    using TestCaseManagement.Api.Models.DTOs.Modules;
    using TestCaseManagement.Api.Models.DTOs.Products;
    using TestCaseManagement.Api.Models.DTOs.TestCases;
    using TestCaseManagement.Api.Models.DTOs.TestRuns;
    using TestCaseManagement.Api.Models.DTOs.TestSuites;
    using TestCaseManagement.Api.Models.DTOs.Uploads;
    using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Api.Models.Responses.Products;

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
            CreateMap<ProductVersion, ProductVersionResponse>();

            // Module mappings
            CreateMap<CreateModuleRequest, Module>();
                CreateMap<Module, ModuleResponse>();
                CreateMap<Module, ModuleWithAttributesResponse>();

            // ModuleAttribute mappings
            CreateMap<ModuleAttributeRequest, ModuleAttribute>()
.ForMember(dest => dest.Id, opt => opt.Ignore())   // ✅ prevent overwriting with null
.ReverseMap();
            CreateMap<ModuleAttribute, ModuleAttributeResponse>();

            // TestSuite mappings
            CreateMap<CreateTestSuiteRequest, TestSuite>();
                CreateMap<TestSuite, TestSuiteResponse>();
                CreateMap<TestSuite, TestSuiteWithCasesResponse>();
                CreateMap<TestSuite, TestRunTestSuiteResponse>(); // Added mapping

                // TestCase mappings
                CreateMap<CreateTestCaseRequest, TestCase>()
                    .ForMember(dest => dest.ProductVersionId, opt => opt.MapFrom(src => src.ProductVersionId));

                CreateMap<UpdateTestCaseRequest, TestCase>()
                    .ForMember(dest => dest.ProductVersionId, opt => opt.MapFrom(src => src.ProductVersionId))
                    .ForMember(dest => dest.TestCaseId, opt => opt.MapFrom(src => src.TestCaseId ?? string.Empty));

            CreateMap<TestCase, TestCaseResponse>()
    .ForMember(dest => dest.Actual, opt => opt.MapFrom(src => src.Actual))
    .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks))
 
    .ForMember(dest => dest.ProductVersionName, // ✅ new mapping
        opt => opt.MapFrom(src => src.ProductVersion != null ? src.ProductVersion.Version : null));
            // ✅ Add this


            CreateMap<TestCase, TestCaseDetailResponse>()
                    .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.ManualTestCaseSteps.Select(s => s.Steps).ToList()))
                    .ForMember(dest => dest.Expected, opt => opt.MapFrom(src => src.ManualTestCaseSteps.Select(s => s.ExpectedResult).ToList()))
                    .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.TestCaseAttributes))
                    .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Uploads))
                    .ForMember(dest => dest.TestSuiteIds, opt => opt.MapFrom(src => src.TestSuiteTestCases.Select(tstc => tstc.TestSuiteId).ToList()))
                    .ForMember(dest => dest.Actual, opt => opt.MapFrom(src => src.Actual))
                    .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks))
                    .ForMember(dest => dest.ProductVersionName,
                        opt => opt.MapFrom(src => src.ProductVersion != null ? src.ProductVersion.Version : null));


                // ManualTestCaseStep mappings
                CreateMap<ManualTestCaseStep, ManualTestCaseStepRequest>().ReverseMap();

                // TestCaseAttribute mappings
                CreateMap<TestCaseAttribute, TestCaseAttributeRequest>().ReverseMap();
                CreateMap<TestCaseAttribute, TestCaseAttributeResponse>()
                    .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.ModuleAttribute.Key))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ModuleAttribute.Name))
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ModuleAttribute.Type))
                    .ForMember(dest => dest.IsRequired, opt => opt.MapFrom(src => src.ModuleAttribute.IsRequired));

                // Upload mappings
                CreateMap<Upload, UploadResponse>()
                    .ForMember(dest => dest.TestCaseId, opt => opt.MapFrom(src => src.TestCaseId))
                    .ForMember(dest => dest.UploadedBy, opt => opt.MapFrom(src => src.UploadedBy))
                    .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FilePath));

                // TestRun mappings
                CreateMap<CreateTestRunRequest, TestRun>();
                CreateMap<TestRun, TestRunResponse>();

                // TestRunResult mappings
                CreateMap<TestRunResult, TestRunResultResponse>();
            }
        }
    }
