namespace TestCaseManagement.Api.Models.DTOs.Common;

public class BaseResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static BaseResponse Ok(string message = "") => new()
    {
        Success = true,
        Message = message
    };

    public static BaseResponse Fail(string message, List<string>? errors = null) => new()
    {
        Success = false,
        Message = message,
        Errors = errors ?? new List<string>()
    };
}