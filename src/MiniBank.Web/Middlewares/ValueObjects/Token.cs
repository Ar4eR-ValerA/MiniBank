using System.Text.Json;

namespace MiniBank.Web.Middlewares.ValueObjects;

public class Token
{
    public Token(string token)
    {
        var encodedPayload = token.Split(".")[1];
        var decodedPayload = Convert.FromBase64String(encodedPayload);

        Payload = JsonSerializer.Deserialize<Payload>(decodedPayload);
    }
    
    public Payload Payload { get; }
}