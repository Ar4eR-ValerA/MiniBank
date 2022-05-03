using System.Text.Json.Serialization;

namespace MiniBank.Web.Middlewares.ValueObjects;

public class Payload
{
    public DateTime ExpirationUtc => DateTime.UnixEpoch.AddSeconds(ExpirationUnix);

    [JsonPropertyName("exp")]
    public int ExpirationUnix { get; init; }
}