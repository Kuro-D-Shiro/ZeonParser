using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ZeonService.Middleware
{
    public class ExceptionToJsonMiddleware(RequestDelegate next/*, ILogger logger*/) 
    {
        private readonly RequestDelegate next = next;
        //private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            //_logger.LogError(exception, "An unexpected error occurred.");

            var (statusCode, title) = exception switch
            {
                ApplicationException _ => (StatusCodes.Status400BadRequest, "Bad request"),
                KeyNotFoundException _ => (StatusCodes.Status404NotFound, "Not found"),
                UnauthorizedAccessException _ => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                _ => (StatusCodes.Status500InternalServerError, "Internal server error")
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Instance = context.Request.Path,
                Extensions =
                {
                    ["traceId"] = context.TraceIdentifier,
                    ["timestamp"] = DateTime.UtcNow
                }
            };

            if (context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
            {
                problemDetails.Detail = exception.ToString();
                problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
