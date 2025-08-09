using System.ComponentModel.DataAnnotations;
using FluentValidation;
using TestCaseManagement.Api.Constants;

namespace TestCaseManagement.Api.Models.DTOs.Products;

public class UpdateProductRequest
{
    [Required(ErrorMessage = ValidationMessages.ProductNameRequired)]
    [MaxLength(100, ErrorMessage = ValidationMessages.ProductNameMaxLength)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = ValidationMessages.ProductDescriptionMaxLength)]
    public string? Description { get; set; }

    public bool? IsActive { get; set; } // Nullable to support partial updates
}

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.ProductNameRequired)
            .MaximumLength(100).WithMessage(ValidationMessages.ProductNameMaxLength);

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage(ValidationMessages.ProductDescriptionMaxLength);

        // Custom validation example
        RuleFor(x => x.Name)
            .Must(name => !name.Contains("test", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Product name cannot contain 'test'");
    }
}