namespace TestCaseManagement.Api.Constants;

public static class AppConstants
{
    public static readonly string[] AllowedTestTypes =
        { "Manual", "Automation", "WebAPI", "Database", "Performance" };

    public static readonly string[] AllowedTestRunStatuses =
        { "Not Started", "In Progress", "Completed", "Blocked" };

    public static readonly string[] AllowedResultStatuses =
        { "Pass", "Fail", "Pending", "Blocked" };
}