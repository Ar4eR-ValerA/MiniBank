using System.Net;
using MiniBank.Web.Middlewares.ValueObjects;

namespace MiniBank.Web.Middlewares;

public class CustomAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public CustomAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        var authorizationHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

        if (authorizationHeader is null)
        {
            await _next(httpContext);
            return;
        }

        var token = new Token(authorizationHeader.Split(" ").Last());
        var expiration = token.Payload.ExpirationUtc;

        if (expiration < DateTime.UtcNow)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await httpContext.Response.WriteAsJsonAsync(new
            {
                Message = $"Your token has expired on {expiration} UTC"
            });
            return;
        }
        
        await _next(httpContext);
    }
}