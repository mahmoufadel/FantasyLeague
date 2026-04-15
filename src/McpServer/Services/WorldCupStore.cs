using McpServer.Models;

namespace McpServer.Services;

/// <summary>
/// In-memory store for World Cup matches and user messages
/// </summary>
public class WorldCupStore
{
    private readonly Dictionary<string, WorldCupMatch> _matches = new();
    private readonly Dictionary<string, UserMessage> _messages = new();
    private readonly Dictionary<string, ReplaySession> _sessions = new();
    private readonly object _lock = new();

    // Match Operations
    public WorldCupMatch CreateMatch(string homeTeam, string awayTeam, DateTime matchDate, TournamentStage stage, string venue, string? group = null)
    {
        lock (_lock)
        {
            var match = new WorldCupMatch(homeTeam, awayTeam, matchDate, stage, venue, group);
            _matches[match.Id] = match;
            return match;
        }
    }

    public WorldCupMatch? GetMatch(string id)
    {
        lock (_lock)
        {
            return _matches.TryGetValue(id, out var match) ? match : null;
        }
    }

    public IReadOnlyList<WorldCupMatch> GetAllMatches()
    {
        lock (_lock)
        {
            return _matches.Values.OrderBy(m => m.MatchDate).ToList();
        }
    }

    public IReadOnlyList<WorldCupMatch> GetMatchesByStage(TournamentStage stage)
    {
        lock (_lock)
        {
            return _matches.Values.Where(m => m.Stage == stage).OrderBy(m => m.MatchDate).ToList();
        }
    }

    public IReadOnlyList<WorldCupMatch> GetMatchesByTeam(string team)
    {
        lock (_lock)
        {
            return _matches.Values
                .Where(m => m.HomeTeam.Equals(team, StringComparison.OrdinalIgnoreCase) ||
                            m.AwayTeam.Equals(team, StringComparison.OrdinalIgnoreCase))
                .OrderBy(m => m.MatchDate)
                .ToList();
        }
    }

    public WorldCupMatch? UpdateMatchScore(string id, int homeScore, int awayScore)
    {
        lock (_lock)
        {
            if (!_matches.TryGetValue(id, out var match))
                return null;

            match.UpdateScore(homeScore, awayScore);
            return match;
        }
    }

    public WorldCupMatch? StartMatch(string id)
    {
        lock (_lock)
        {
            if (!_matches.TryGetValue(id, out var match))
                return null;

            match.StartMatch();
            return match;
        }
    }

    // Message Operations
    public UserMessage AddMessage(string userId, string username, string matchId, string content, MessageType type)
    {
        lock (_lock)
        {
            var message = new UserMessage(userId, username, matchId, content, type);
            _messages[message.Id] = message;
            return message;
        }
    }

    public UserMessage? GetMessage(string id)
    {
        lock (_lock)
        {
            return _messages.TryGetValue(id, out var message) ? message : null;
        }
    }

    public IReadOnlyList<UserMessage> GetMessagesForMatch(string matchId)
    {
        lock (_lock)
        {
            return _messages.Values
                .Where(m => m.MatchId == matchId)
                .OrderBy(m => m.Timestamp)
                .ToList();
        }
    }

    public IReadOnlyList<UserMessage> GetMessagesByUser(string userId)
    {
        lock (_lock)
        {
            return _messages.Values
                .Where(m => m.UserId == userId)
                .OrderBy(m => m.Timestamp)
                .ToList();
        }
    }

    public bool DeleteMessage(string id)
    {
        lock (_lock)
        {
            return _messages.Remove(id);
        }
    }

    // Replay Session Operations
    public ReplaySession? CreateReplaySession(string matchId)
    {
        lock (_lock)
        {
            if (!_matches.ContainsKey(matchId))
                return null;

            var messages = GetMessagesForMatch(matchId).ToList();
            if (messages.Count == 0)
                return null;

            var session = new ReplaySession(matchId, messages);
            _sessions[session.SessionId] = session;
            return session;
        }
    }

    public ReplaySession? GetSession(string sessionId)
    {
        lock (_lock)
        {
            return _sessions.TryGetValue(sessionId, out var session) ? session : null;
        }
    }

    public ReplaySession? StartReplay(string sessionId, double speed = 1.0)
    {
        lock (_lock)
        {
            if (!_sessions.TryGetValue(sessionId, out var session))
                return null;

            session.IsPlaying = true;
            session.Speed = speed;
            session.StartedAt ??= DateTime.UtcNow;
            return session;
        }
    }

    public ReplaySession? PauseReplay(string sessionId)
    {
        lock (_lock)
        {
            if (!_sessions.TryGetValue(sessionId, out var session))
                return null;

            session.IsPlaying = false;
            return session;
        }
    }

    public ReplaySession? StopReplay(string sessionId)
    {
        lock (_lock)
        {
            if (!_sessions.TryGetValue(sessionId, out var session))
                return null;

            session.IsPlaying = false;
            session.CurrentIndex = 0;
            session.EndedAt = DateTime.UtcNow;
            return session;
        }
    }

    public (ReplaySession Session, UserMessage? Message)? GetNextMessage(string sessionId)
    {
        lock (_lock)
        {
            if (!_sessions.TryGetValue(sessionId, out var session))
                return null;

            var message = session.GetNextMessage();
            if (message == null)
            {
                session.IsPlaying = false;
                session.EndedAt = DateTime.UtcNow;
            }

            return (session, message);
        }
    }

    public ReplaySession? ResetReplay(string sessionId)
    {
        lock (_lock)
        {
            if (!_sessions.TryGetValue(sessionId, out var session))
                return null;

            session.Reset();
            return session;
        }
    }

    public ReplaySessionStatus? GetReplayStatus(string sessionId)
    {
        lock (_lock)
        {
            if (!_sessions.TryGetValue(sessionId, out var session))
                return null;

            return new ReplaySessionStatus
            {
                SessionId = session.SessionId,
                MatchId = session.MatchId,
                IsPlaying = session.IsPlaying,
                CurrentIndex = session.CurrentIndex,
                TotalMessages = session.Messages.Count,
                Progress = session.GetProgress(),
                CurrentMessage = session.PeekNextMessage()
            };
        }
    }

    // Stats
    public (int TotalMatches, int TotalMessages, int CompletedMatches, int InProgressMatches) GetStats()
    {
        lock (_lock)
        {
            var totalMatches = _matches.Count;
            var totalMessages = _messages.Count;
            var completed = _matches.Values.Count(m => m.Status == MatchStatus.Completed);
            var inProgress = _matches.Values.Count(m => m.Status == MatchStatus.InProgress);

            return (totalMatches, totalMessages, completed, inProgress);
        }
    }

    // Seed Data
    public void SeedSampleData()
    {
        // Group Stage Matches
        var matches = new[]
        {
            ("USA", "Mexico", "2026-06-12T16:00:00Z", TournamentStage.Group, "SoFi Stadium, Los Angeles", "A"),
            ("Brazil", "Argentina", "2026-06-13T14:00:00Z", TournamentStage.Group, "MetLife Stadium, New York", "B"),
            ("Germany", "France", "2026-06-13T17:00:00Z", TournamentStage.Group, "AT&T Stadium, Dallas", "C"),
            ("Spain", "Portugal", "2026-06-14T16:00:00Z", TournamentStage.Group, "Hard Rock Stadium, Miami", "D"),
            ("England", "Italy", "2026-06-14T19:00:00Z", TournamentStage.Group, "Mercedes-Benz Stadium, Atlanta", "E")
        };

        foreach (var (home, away, date, stage, venue, group) in matches)
        {
            CreateMatch(home, away, DateTime.Parse(date), stage, venue, group);
        }

        // Add sample messages for USA vs Mexico match
        var usaMexicoMatch = GetAllMatches().FirstOrDefault(m => m.HomeTeam == "USA" && m.AwayTeam == "Mexico");
        if (usaMexicoMatch != null)
        {
            AddMessage("user_1", "SoccerFan99", usaMexicoMatch.Id,
                "What a matchup! USA vs Mexico opening game is going to be electric!", MessageType.Commentary);

            AddMessage("user_2", "WorldCupExpert", usaMexicoMatch.Id,
                "I predict USA will win 2-1. Home advantage is huge in World Cup openers.", MessageType.Prediction);

            AddMessage("user_3", "MexicoSupporter", usaMexicoMatch.Id,
                "Vamos Mexico! This is our time to shine!", MessageType.Reaction);

            AddMessage("user_1", "SoccerFan99", usaMexicoMatch.Id,
                "The atmosphere at SoFi Stadium is incredible right now!", MessageType.Commentary);

            AddMessage("user_4", "TacticalAnalyst", usaMexicoMatch.Id,
                "Looking at the formations, USA is going with a 4-3-3 while Mexico is sticking to their traditional 4-4-2.", MessageType.Analysis);
        }
    }
}
