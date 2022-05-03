using System.Text.Json;

namespace MiniBank.Web.Middlewares.ValueObjects;

public class Token
{
    public Token(string token)
    {
        var encodedSections = token.Split(".");
        
        var decodedPayload = Convert.FromBase64String(encodedSections[1]);

        Payload = JsonSerializer.Deserialize<Payload>(decodedPayload);
    }
    
    public Payload Payload { get; }
}