using System.Text.Json.Serialization;

namespace McpServer.Models;

/// <summary>
/// Represents a user message about a World Cup match
/// </summary>
public class UserMessage
{
    public string Id { get; private set; }
    public string UserId { get; private set; }
    public string Username { get; private set; }
    public string MatchId { get; private set; }
    public string Content { get; private set; }
    public MessageType Type { get; private set; }
    public DateTime Timestamp { get; private set; }

    public UserMessage(string userId, string username, string matchId, string content, MessageType type)
    {
        Id = $"msg_{DateTime.UtcNow.Ticks}_{Guid.NewGuid().ToString()[..8]}";
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Username = username ?? throw new ArgumentNullException(nameof(username));
        MatchId = matchId ?? throw new ArgumentNullException(nameof(matchId));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Type = type;
        Timestamp = DateTime.UtcNow;
    }

    public override string ToString()
    {
        return $"[{Timestamp:HH:mm:ss}] {Username} ({Type}): {Content}";
    }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MessageType
{
    Prediction,
    Commentary,
    Reaction,
    Analysis
}
