using System.Net;
using FluentValidation;
using MiniBank.Core.Tools;

namespace MiniBank.Web.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ValidationException exception)
            {
                var errors = exception.Errors
                    .Select(e => $"{e.ErrorMessage}");
                var errorsMessage = string.Join(Environment.NewLine, errors);

                await httpContext.Response.WriteAsJsonAsync(new
                {
                    StatusCode = HttpStatusCode.BadRequest, errorsMessage
                });
            }
            catch (UserFriendlyException exception)
            {
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    StatusCode = HttpStatusCode.BadRequest, exception.Message
                });
            }
            catch (Exception exception)
            {
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    statusCode = HttpStatusCode.InternalServerError
                });

                Console.WriteLine(exception.Message);
            }
        }
    }
}