using FluentValidation;
using TestCaseManagement.Api.Constants;

namespace TestCaseManagement.Api.Models.DTOs.TestCases;

public class UpdateTestCaseRequest
{
    public string? ProductVersionId { get; set; }  // Added version FK property

    public string? UseCase { get; set; }
    public string? Scenario { get; set; }
    public string? TestType { get; set; }
    public string? TestTool { get; set; }
    public string? Result { get; set; }
    public string? Actual { get; set; }
    public string? Remarks { get; set; }
}

public class UpdateTestCaseRequestValidator : AbstractValidator<UpdateTestCaseRequest>
{
    public UpdateTestCaseRequestValidator()
    {
        RuleFor(x => x.TestType)
            .Must(t => t == null || AppConstants.AllowedTestTypes.Contains(t))
            .WithMessage($"Test type must be one of: {string.Join(", ", AppConstants.AllowedTestTypes)}");

        RuleFor(x => x.Result)
            .Must(r => r == null || AppConstants.AllowedResultStatuses.Contains(r))
            .WithMessage($"Result must be one of: {string.Join(", ", AppConstants.AllowedResultStatuses)}");

        RuleFor(x => x.TestTool)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.TestTool))
            .WithMessage("Test tool cannot exceed 100 characters");
    }
}
