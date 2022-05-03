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

                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    Message = errorsMessage
                });
            }
            catch (UserFriendlyException exception)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    Message = exception.Message
                });
            }
            catch (Exception exception)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                Console.WriteLine(exception.Message);
            }
        }
    }
}