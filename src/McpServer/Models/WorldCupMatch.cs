using System.Text.Json.Serialization;

namespace McpServer.Models;

/// <summary>
/// Represents a World Cup match
/// </summary>
public class WorldCupMatch
{
    public string Id { get; private set; }
    public string HomeTeam { get; private set; }
    public string AwayTeam { get; private set; }
    public DateTime MatchDate { get; private set; }
    public string? Group { get; private set; }
    public TournamentStage Stage { get; private set; }
    public int? HomeScore { get; private set; }
    public int? AwayScore { get; private set; }
    public MatchStatus Status { get; private set; }
    public string Venue { get; private set; }

    public WorldCupMatch(string homeTeam, string awayTeam, DateTime matchDate, TournamentStage stage, string venue, string? group = null)
    {
        Id = $"match_{DateTime.UtcNow.Ticks}_{Guid.NewGuid().ToString()[..8]}";
        HomeTeam = homeTeam ?? throw new ArgumentNullException(nameof(homeTeam));
        AwayTeam = awayTeam ?? throw new ArgumentNullException(nameof(awayTeam));
        MatchDate = matchDate;
        Stage = stage;
        Venue = venue ?? throw new ArgumentNullException(nameof(venue));
        Group = group;
        Status = MatchStatus.Scheduled;
    }

    public void UpdateScore(int homeScore, int awayScore)
    {
        HomeScore = homeScore;
        AwayScore = awayScore;
        Status = MatchStatus.Completed;
    }

    public void StartMatch()
    {
        Status = MatchStatus.InProgress;
    }

    public override string ToString()
    {
        var score = Status == MatchStatus.Completed && HomeScore.HasValue && AwayScore.HasValue
            ? $" ({HomeScore}-{AwayScore})"
            : "";
        return $"{HomeTeam} vs {AwayTeam}{score} - {Stage} - {MatchDate:yyyy-MM-dd HH:mm}";
    }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TournamentStage
{
    Group,
    RoundOf16,
    QuarterFinal,
    SemiFinal,
    ThirdPlace,
    Final
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MatchStatus
{
    Scheduled,
    InProgress,
    Completed
}
