using System.Net;
using System.Text.Json.Nodes;

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

        var token = authorizationHeader.Split(" ").Last();
        var expiration = GetExpirationDate(token);

        if (expiration < DateTime.UtcNow)
        {
            await httpContext.Response.WriteAsJsonAsync(new
            {
                StatusCode = HttpStatusCode.Forbidden, 
                Message = $"Your token has expired on {expiration} UTC"
            });
            return;
        }
        
        await _next(httpContext);
    }

    private DateTime GetExpirationDate(string encodedToken)
    {
        var encodedPayload = encodedToken.Split(".")[1];
        var decodedPayload = Convert.FromBase64String(encodedPayload);

        var payloadJson = JsonNode.Parse(decodedPayload);
        var expirationUnixTime = (int)payloadJson?["exp"];

        return DateTime.UnixEpoch.AddSeconds(expirationUnixTime);
    }
}