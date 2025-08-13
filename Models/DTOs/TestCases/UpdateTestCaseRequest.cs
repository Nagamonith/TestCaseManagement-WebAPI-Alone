using FluentValidation;
using TestCaseManagement.Api.Constants;
using System.Collections.Generic;

namespace TestCaseManagement.Api.Models.DTOs.TestCases
{
    public class UpdateTestCaseRequest
    {
        public string? ProductVersionId { get; set; }
        public string? UseCase { get; set; }
        public string? Scenario { get; set; }
        public string? TestType { get; set; }
        public string? TestTool { get; set; }
        public string? Result { get; set; }
        public string? Actual { get; set; }
        public string? Remarks { get; set; }
        public List<ManualTestCaseStepRequest>? Steps { get; set; }

        // NEW property to include attributes
        public List<TestCaseAttributeRequest>? Attributes { get; set; }
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

            When(x => x.Steps != null, () =>
            {
                RuleForEach(x => x.Steps).ChildRules(step =>
                {
                    step.RuleFor(s => s.Steps)
                        .NotEmpty()
                        .WithMessage("Step description is required");

                    step.RuleFor(s => s.ExpectedResult)
                        .NotEmpty()
                        .WithMessage("Expected result is required");
                });
            });

            // NEW validation for attributes
            When(x => x.Attributes != null, () =>
            {
                RuleForEach(x => x.Attributes).ChildRules(attr =>
                {
                    attr.RuleFor(a => a.Key)
                        .NotEmpty()
                        .WithMessage("Attribute key is required");

                    attr.RuleFor(a => a.Value)
                        .NotEmpty()
                        .WithMessage("Attribute value is required");
                });
            });
        }
    }
}
