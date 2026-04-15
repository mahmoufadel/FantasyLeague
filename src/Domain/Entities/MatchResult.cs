using Domain.Common;

namespace Domain.Entities;

public class MatchResult : Entity
{
    public DateTime MatchDate { get; private set; }
    public Guid HomeTeamId { get; private set; }
    public Guid AwayTeamId { get; private set; }
    public int HomeScore { get; private set; }
    public int AwayScore { get; private set; }

    private MatchResult() { } // For EF Core

    public MatchResult(DateTime matchDate, Guid homeTeamId, Guid awayTeamId, int homeScore, int awayScore)
    {
        if (homeTeamId == Guid.Empty)
            throw new ArgumentException("HomeTeamId cannot be empty", nameof(homeTeamId));
        if (awayTeamId == Guid.Empty)
            throw new ArgumentException("AwayTeamId cannot be empty", nameof(awayTeamId));
        if (homeTeamId == awayTeamId)
            throw new ArgumentException("Home team and away team cannot be the same");
        if (homeScore < 0)
            throw new ArgumentException("Home score cannot be negative", nameof(homeScore));
        if (awayScore < 0)
            throw new ArgumentException("Away score cannot be negative", nameof(awayScore));

        MatchDate = matchDate;
        HomeTeamId = homeTeamId;
        AwayTeamId = awayTeamId;
        HomeScore = homeScore;
        AwayScore = awayScore;
    }
}
