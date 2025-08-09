namespace TestCaseManagement.Api.Constants;

public static class ValidationMessages
{
    public const string RequiredField = "{PropertyName} is required";
    public const string MaxLength = "{PropertyName} cannot exceed {MaxLength} characters";
    public const string InvalidTestType = "Test type must be one of: {AllowedTypes}";
    public const string ProductNameRequired = "Product name is required.";
    public const string ProductNameMaxLength = "Product name cannot exceed 100 characters.";
    public const string ProductDescriptionMaxLength = "Product description cannot exceed 500 characters.";
}