using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
            catch (Exception exception)
            {
                await httpContext.Response.WriteAsJsonAsync(new { Message = "Internal error" });
            }
        }
    }
}