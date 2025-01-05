using BL.Interfaces;
using Newtonsoft.Json;
using System.Net;

namespace GitRepositoriesApi.Middelwares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IAuthManager _authManager;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IAuthManager authManager)
        {
            _next = next;
            _logger = logger;
            _authManager = authManager;

        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            HttpStatusCode statusCode;
            string errorType;
            string message;

            switch (exception)
            {
                case ArgumentException _:
                    statusCode = HttpStatusCode.BadRequest;
                    errorType = "BadRequest";
                    message = "Invalid input provided.";
                    break;
                case KeyNotFoundException _:
                    statusCode = HttpStatusCode.NotFound;
                    errorType = "NotFound";
                    message = "The requested resource was not found.";
                    break;
                case UnauthorizedAccessException _:
                    statusCode = HttpStatusCode.Unauthorized;
                    errorType = "Unauthorized";
                    message = "Invalid or expired token.";
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    errorType = "InternalServerError";
                    message = "An unexpected error occurred.";
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                StatusCode = context.Response.StatusCode,
                Error = errorType,
                Message = message,
                DetailedMessage = exception.Message
            }));
        }
    }
}

