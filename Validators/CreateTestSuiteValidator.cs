using FluentValidation;
using TestCaseManagement.Api.Models.DTOs.TestSuites;

namespace TestCaseManagement.Api.Validators;

public class CreateTestSuiteValidator : AbstractValidator<CreateTestSuiteRequest>
{
    public CreateTestSuiteValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Test suite name is required")
            .MaximumLength(100).WithMessage("Test suite name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed 500 characters");
    }
}