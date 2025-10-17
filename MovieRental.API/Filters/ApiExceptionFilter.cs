using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MovieRental.API.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;
        private readonly IHostEnvironment _env;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An unhandled exception has occurred");

            var error = new
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = _env.IsDevelopment() 
                    ? context.Exception.Message 
                    : "An error occurred while processing your request.",
                Details = _env.IsDevelopment() ? context.Exception.StackTrace : null
            };

            context.Result = new ObjectResult(error)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true;
        }
    }
}
