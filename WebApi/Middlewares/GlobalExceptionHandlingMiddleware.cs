using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using System.Text.Json;
using Core.Models;

namespace WebApi.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _request;
        private readonly Serilog.ILogger _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate request, Serilog.ILogger logger)
        {
            _request = request;
            _logger = logger.ForContext<GlobalExceptionHandlingMiddleware>();
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var request = $"{httpContext.Request.Method}: {httpContext.Request.Path}";
            try
            {
                _logger?.Information("{@request} received", request);
                await _request(httpContext);
                _logger?.Information("{@request} completed", request);
            }
            catch (Exception ex)
            {
                _logger?.Error("{@request} failed", request);
                _logger?.Error(ex, ex.Message);
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(ServiceResult<object>.Failure(HttpStatusCode.InternalServerError, ex.Message)));
            }
        }
    }
}
