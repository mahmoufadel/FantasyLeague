namespace McpServer.Models;

/// <summary>
/// Represents a replay session for user messages about a match
/// </summary>
public class ReplaySession
{
    public string SessionId { get; private set; }
    public string MatchId { get; private set; }
    public IReadOnlyList<UserMessage> Messages { get; private set; }
    public int CurrentIndex { get; set; }
    public bool IsPlaying { get; set; }
    public double Speed { get; set; } // Messages per second
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    public ReplaySession(string matchId, List<UserMessage> messages)
    {
        SessionId = $"session_{DateTime.UtcNow.Ticks}_{Guid.NewGuid().ToString()[..8]}";
        MatchId = matchId ?? throw new ArgumentNullException(nameof(matchId));
        Messages = messages ?? throw new ArgumentNullException(nameof(messages));
        CurrentIndex = 0;
        IsPlaying = false;
        Speed = 1.0;
    }

    public UserMessage? GetNextMessage()
    {
        if (CurrentIndex >= Messages.Count)
            return null;

        return Messages[CurrentIndex++];
    }

    public UserMessage? PeekNextMessage()
    {
        if (CurrentIndex >= Messages.Count)
            return null;

        return Messages[CurrentIndex];
    }

    public double GetProgress()
    {
        if (Messages.Count == 0)
            return 100;

        return (CurrentIndex / (double)Messages.Count) * 100;
    }

    public bool IsComplete => CurrentIndex >= Messages.Count;

    public void Reset()
    {
        CurrentIndex = 0;
        IsPlaying = false;
        EndedAt = null;
        StartedAt = null;
    }
}

/// <summary>
/// Status DTO for replay session
/// </summary>
public class ReplaySessionStatus
{
    public string SessionId { get; set; } = string.Empty;
    public string MatchId { get; set; } = string.Empty;
    public bool IsPlaying { get; set; }
    public int CurrentIndex { get; set; }
    public int TotalMessages { get; set; }
    public double Progress { get; set; }
    public UserMessage? CurrentMessage { get; set; }
}
