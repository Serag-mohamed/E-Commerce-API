using System.Net;

namespace E_Commerce.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware
            (
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment env
            )
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Non expect error: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var statusCode = ex switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound,
                ArgumentException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError,
            };

            context.Response.StatusCode = (int)statusCode;

            string message = string.Empty;
            string detail = string.Empty;

            if(statusCode == HttpStatusCode.InternalServerError)
            {
                message = "Sorry, An error occurred on the server. Please try again later.";

                if (_env.IsDevelopment())
                    detail = ex.ToString();
            }
            else
                message = ex.Message;

            var response = new
            {
                statusCode = context.Response.StatusCode,
                Message = message,
                Detail = detail
            };
            await context.Response.WriteAsJsonAsync(response);
            
        }
    }
}
