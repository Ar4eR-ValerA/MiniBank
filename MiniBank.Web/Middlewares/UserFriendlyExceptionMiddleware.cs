using System.Net;
using MiniBank.Core.Tools;

namespace MiniBank.Web.Middlewares
{
    public class UserFriendlyExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public UserFriendlyExceptionMiddleware(RequestDelegate next)
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
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    StatusCode = HttpStatusCode.BadRequest, exception.Message
                });
            }
        }
    }
}