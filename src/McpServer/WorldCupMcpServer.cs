using System.ComponentModel;
using System.Text.Json;
using McpServer.Models;
using McpServer.Services;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace McpServer;

/// <summary>
/// MCP Server for World Cup matches and user message replay
/// </summary>
[McpServerToolType]
public class WorldCupMcpServer
{
    private readonly WorldCupStore _store;

    public WorldCupMcpServer()
    {
        _store = new WorldCupStore();
        _store.SeedSampleData();
    }

    public WorldCupMcpServer(WorldCupStore store)
    {
        _store = store;
    }

    // ============================================
    // Match Tools
    // ============================================

    [McpServerTool, Description("Get all World Cup matches ordered by date")]
    public string GetAllMatches()
    {
        var matches = _store.GetAllMatches();
        return JsonSerializer.Serialize(matches, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Get a specific World Cup match by ID")]
    public string GetMatchById(
        [Description("The ID of the match to retrieve")] string matchId)
    {
        var match = _store.GetMatch(matchId);
        if (match == null)
            return $"Match with ID '{matchId}' not found.";

        return JsonSerializer.Serialize(match, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Get World Cup matches filtered by tournament stage")]
    public string GetMatchesByStage(
        [Description("The tournament stage (Group, RoundOf16, QuarterFinal, SemiFinal, ThirdPlace, Final)")] string stage)
    {
        if (!Enum.TryParse<TournamentStage>(stage, true, out var tournamentStage))
        {
            return $"Invalid stage '{stage}'. Valid values are: Group, RoundOf16, QuarterFinal, SemiFinal, ThirdPlace, Final";
        }

        var matches = _store.GetMatchesByStage(tournamentStage);
        return JsonSerializer.Serialize(matches, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Get matches for a specific team")]
    public string GetMatchesByTeam(
        [Description("The team name to search for")] string teamName)
    {
        var matches = _store.GetMatchesByTeam(teamName);
        return JsonSerializer.Serialize(matches, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Create a new World Cup match")]
    public string CreateMatch(
        [Description("Home team name")] string homeTeam,
        [Description("Away team name")] string awayTeam,
        [Description("Match date in ISO format (e.g., 2026-06-15T18:00:00Z)")] string matchDate,
        [Description("Tournament stage (Group, RoundOf16, QuarterFinal, SemiFinal, ThirdPlace, Final)")] string stage,
        [Description("Venue/stadium name")] string venue,
        [Description("Group name (optional, for group stage)")] string? group = null)
    {
        try
        {
            if (!Enum.TryParse<TournamentStage>(stage, true, out var tournamentStage))
            {
                return $"Invalid stage '{stage}'. Valid values are: Group, RoundOf16, QuarterFinal, SemiFinal, ThirdPlace, Final";
            }

            var date = DateTime.Parse(matchDate);
            var match = _store.CreateMatch(homeTeam, awayTeam, date, tournamentStage, venue, group);

            return $"Match created successfully!\n\n{JsonSerializer.Serialize(match, new JsonSerializerOptions { WriteIndented = true })}";
        }
        catch (Exception ex)
        {
            return $"Error creating match: {ex.Message}";
        }
    }

    [McpServerTool, Description("Update the score for a match")]
    public string UpdateMatchScore(
        [Description("The match ID")] string matchId,
        [Description("Home team score")] int homeScore,
        [Description("Away team score")] int awayScore)
    {
        var match = _store.UpdateMatchScore(matchId, homeScore, awayScore);
        if (match == null)
            return $"Match with ID '{matchId}' not found.";

        return $"Score updated!\n\n{JsonSerializer.Serialize(match, new JsonSerializerOptions { WriteIndented = true })}";
    }

    [McpServerTool, Description("Mark a match as in-progress")]
    public string StartMatch(
        [Description("The match ID")] string matchId)
    {
        var match = _store.StartMatch(matchId);
        if (match == null)
            return $"Match with ID '{matchId}' not found.";

        return $"Match started!\n\n{JsonSerializer.Serialize(match, new JsonSerializerOptions { WriteIndented = true })}";
    }

    // ============================================
    // Message Tools
    // ============================================

    [McpServerTool, Description("Add a user message/commentary/prediction for a match")]
    public string AddUserMessage(
        [Description("Unique user ID")] string userId,
        [Description("Display username")] string username,
        [Description("Match ID to comment on")] string matchId,
        [Description("The message content")] string message,
        [Description("Type of message (Prediction, Commentary, Reaction, Analysis)")] string messageType)
    {
        try
        {
            if (!Enum.TryParse<MessageType>(messageType, true, out var type))
            {
                return $"Invalid message type '{messageType}'. Valid values are: Prediction, Commentary, Reaction, Analysis";
            }

            var match = _store.GetMatch(matchId);
            if (match == null)
                return $"Match with ID '{matchId}' not found.";

            var userMessage = _store.AddMessage(userId, username, matchId, message, type);
            return $"Message added!\n\n{userMessage}";
        }
        catch (Exception ex)
        {
            return $"Error adding message: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get all user messages for a specific match")]
    public string GetMessagesForMatch(
        [Description("The match ID")] string matchId)
    {
        var match = _store.GetMatch(matchId);
        if (match == null)
            return $"Match with ID '{matchId}' not found.";

        var messages = _store.GetMessagesForMatch(matchId);
        if (messages.Count == 0)
            return $"No messages found for match '{match}'.";

        return JsonSerializer.Serialize(messages, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Get all messages by a specific user")]
    public string GetMessagesByUser(
        [Description("The user ID")] string userId)
    {
        var messages = _store.GetMessagesByUser(userId);
        return JsonSerializer.Serialize(messages, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Delete a user message")]
    public string DeleteMessage(
        [Description("The message ID to delete")] string messageId)
    {
        var deleted = _store.DeleteMessage(messageId);
        return deleted ? "Message deleted successfully." : $"Message with ID '{messageId}' not found.";
    }

    // ============================================
    // Replay Tools
    // ============================================

    [McpServerTool, Description("Create a replay session to replay user messages for a match")]
    public string CreateReplaySession(
        [Description("The match ID to replay messages for")] string matchId)
    {
        var match = _store.GetMatch(matchId);
        if (match == null)
            return $"Match with ID '{matchId}' not found.";

        var session = _store.CreateReplaySession(matchId);
        if (session == null)
            return $"Cannot create replay session. Match '{match}' has no messages.";

        return $"Replay session created!\n\nSession ID: {session.SessionId}\nTotal Messages: {session.Messages.Count}\nMatch: {match}";
    }

    [McpServerTool, Description("Start or resume a replay session")]
    public string StartReplay(
        [Description("The replay session ID")] string sessionId,
        [Description("Playback speed (messages per second, default: 1)")] double speed = 1.0)
    {
        var session = _store.StartReplay(sessionId, speed);
        if (session == null)
            return $"Session with ID '{sessionId}' not found.";

        return $"Replay started at {speed}x speed!\nProgress: {session.GetProgress():F1}%";
    }

    [McpServerTool, Description("Pause a replay session")]
    public string PauseReplay(
        [Description("The replay session ID")] string sessionId)
    {
        var session = _store.PauseReplay(sessionId);
        if (session == null)
            return $"Session with ID '{sessionId}' not found.";

        return "Replay paused.";
    }

    [McpServerTool, Description("Stop and reset a replay session")]
    public string StopReplay(
        [Description("The replay session ID")] string sessionId)
    {
        var session = _store.StopReplay(sessionId);
        if (session == null)
            return $"Session with ID '{sessionId}' not found.";

        return "Replay stopped and reset to beginning.";
    }

    [McpServerTool, Description("Reset a replay session to the beginning")]
    public string ResetReplay(
        [Description("The replay session ID")] string sessionId)
    {
        var session = _store.ResetReplay(sessionId);
        if (session == null)
            return $"Session with ID '{sessionId}' not found.";

        return "Replay reset to beginning.";
    }

    [McpServerTool, Description("Get the next message in a replay session")]
    public string GetNextMessage(
        [Description("The replay session ID")] string sessionId)
    {
        var result = _store.GetNextMessage(sessionId);
        if (result == null)
            return $"Session with ID '{sessionId}' not found.";

        var (session, message) = result.Value;

        if (message == null)
        {
            return "Replay completed! All messages have been replayed.";
        }

        var progress = session.GetProgress();
        return $"[{progress:F1}%] {message.Username} ({message.Type}): {message.Content}";
    }

    [McpServerTool, Description("Get the current status of a replay session")]
    public string GetReplayStatus(
        [Description("The replay session ID")] string sessionId)
    {
        var status = _store.GetReplayStatus(sessionId);
        if (status == null)
            return $"Session with ID '{sessionId}' not found.";

        return JsonSerializer.Serialize(status, new JsonSerializerOptions { WriteIndented = true });
    }

    // ============================================
    // Stats Tools
    // ============================================

    [McpServerTool, Description("Get tournament statistics")]
    public string GetTournamentStats()
    {
        var stats = _store.GetStats();
        return JsonSerializer.Serialize(new
        {
            TotalMatches = stats.TotalMatches,
            TotalMessages = stats.TotalMessages,
            CompletedMatches = stats.CompletedMatches,
            InProgressMatches = stats.InProgressMatches,
            ScheduledMatches = stats.TotalMatches - stats.CompletedMatches - stats.InProgressMatches
        }, new JsonSerializerOptions { WriteIndented = true });
    }
}
