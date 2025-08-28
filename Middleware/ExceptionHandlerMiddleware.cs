using System.Net;
using TestCaseManagementService.Models.DTOs.Common;
using TestCaseManagementService.Models;

namespace TestCaseManagement.Api.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "Resource not found");
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsJsonAsync(new BaseResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid argument");
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new BaseResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new BaseResponse
            {
                Success = false,
                Message = "An unexpected error occurred"
            });
        }
    }
}