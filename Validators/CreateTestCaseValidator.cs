using FluentValidation;
using TestCaseManagement.Api.Constants;
using TestCaseManagement.Api.Models.DTOs.TestCases;

namespace TestCaseManagement.Api.Validators;

public class CreateTestCaseValidator : AbstractValidator<CreateTestCaseRequest>
{
    public CreateTestCaseValidator()
    {
        RuleFor(x => x.ModuleId)
            .NotEmpty().WithMessage("Module ID is required");

        RuleFor(x => x.ProductVersionId)  // Validate ProductVersionId instead of Version
            .NotEmpty().WithMessage("Product version ID is required")
            .MaximumLength(50).WithMessage("Product version ID cannot exceed 50 characters");

        RuleFor(x => x.TestCaseId)
            .NotEmpty().WithMessage("Test case ID is required")
            .MaximumLength(50).WithMessage("Test case ID cannot exceed 50 characters");

        RuleFor(x => x.UseCase)
            .NotEmpty().WithMessage("Use case is required");

        RuleFor(x => x.Scenario)
            .NotEmpty().WithMessage("Scenario is required");

        RuleFor(x => x.TestType)
            .NotEmpty().WithMessage("Test type is required")
            .Must(x => AppConstants.AllowedTestTypes.Contains(x))
            .WithMessage($"Test type must be one of: {string.Join(", ", AppConstants.AllowedTestTypes)}");

        RuleFor(x => x.TestTool)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.TestTool))
            .WithMessage("Test tool cannot exceed 100 characters");
    }
}
