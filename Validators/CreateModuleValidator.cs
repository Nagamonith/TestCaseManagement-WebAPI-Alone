using FluentValidation;
using TestCaseManagement.Api.Models.DTOs.Modules;

namespace TestCaseManagement.Api.Validators;

public class CreateModuleValidator : AbstractValidator<CreateModuleRequest>
{
    public CreateModuleValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.Version)
            .NotEmpty().WithMessage("Version is required")
            .MaximumLength(20).WithMessage("Version cannot exceed 20 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Module name is required")
            .MaximumLength(100).WithMessage("Module name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed 500 characters");
    }
}