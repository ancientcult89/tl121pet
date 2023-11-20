using System.Net;
using tl121pet.Entities.Infrastructure.Exceptions;
using tl121pet.Infrastructure;

namespace tl121pet.Middlwares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unexpected error occurred.");
       
            //Article of this realisation: https://dev.to/andytechdev/aspnet-core-middleware-working-with-global-exception-handling-3hi0
            ExceptionResponse response = exception switch
            {
                LogicException _ => new ExceptionResponse(HttpStatusCode.BadRequest, exception.Message),
                DataFoundException _ => new ExceptionResponse(HttpStatusCode.NotFound, exception.Message),
                _ => new ExceptionResponse(HttpStatusCode.InternalServerError, exception.Message)
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)response.StatusCode;
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
