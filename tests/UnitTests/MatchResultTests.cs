using Domain.Entities;
using FluentAssertions;

namespace UnitTests.Domain.Entities;

public class MatchResultTests
{
    [Theory]
    [InlineData("2024-01-15", 2, 1)]
    [InlineData("2024-03-20", 0, 0)]
    [InlineData("2024-12-25", 5, 3)]
    public void Constructor_WhenValidData_ShouldCreateMatchResult(string dateStr, int homeScore, int awayScore)
    {
        // Arrange
        var matchDate = DateTime.Parse(dateStr);
        var homeTeamId = Guid.NewGuid();
        var awayTeamId = Guid.NewGuid();

        // Act
        var matchResult = new MatchResult(matchDate, homeTeamId, awayTeamId, homeScore, awayScore);

        // Assert
        matchResult.Should().NotBeNull();
        matchResult.Id.Should().NotBeEmpty();
        matchResult.MatchDate.Should().Be(matchDate);
        matchResult.HomeTeamId.Should().Be(homeTeamId);
        matchResult.AwayTeamId.Should().Be(awayTeamId);
        matchResult.HomeScore.Should().Be(homeScore);
        matchResult.AwayScore.Should().Be(awayScore);
    }

    [Fact]
    public void Constructor_WhenHomeTeamIdIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        var matchDate = DateTime.UtcNow;
        var awayTeamId = Guid.NewGuid();

        // Act
        Action act = () => new MatchResult(matchDate, Guid.Empty, awayTeamId, 1, 0);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*HomeTeamId cannot be empty*");
    }

    [Fact]
    public void Constructor_WhenAwayTeamIdIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        var matchDate = DateTime.UtcNow;
        var homeTeamId = Guid.NewGuid();

        // Act
        Action act = () => new MatchResult(matchDate, homeTeamId, Guid.Empty, 1, 0);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*AwayTeamId cannot be empty*");
    }

    [Fact]
    public void Constructor_WhenTeamsAreSame_ShouldThrowArgumentException()
    {
        // Arrange
        var matchDate = DateTime.UtcNow;
        var teamId = Guid.NewGuid();

        // Act
        Action act = () => new MatchResult(matchDate, teamId, teamId, 1, 0);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Home team and away team cannot be the same*");
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(-5, 2)]
    [InlineData(-100, 1)]
    public void Constructor_WhenHomeScoreIsNegative_ShouldThrowArgumentException(int homeScore, int awayScore)
    {
        // Arrange
        var matchDate = DateTime.UtcNow;
        var homeTeamId = Guid.NewGuid();
        var awayTeamId = Guid.NewGuid();

        // Act
        Action act = () => new MatchResult(matchDate, homeTeamId, awayTeamId, homeScore, awayScore);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Home score cannot be negative*");
    }

    [Theory]
    [InlineData(0, -1)]
    [InlineData(2, -5)]
    [InlineData(1, -100)]
    public void Constructor_WhenAwayScoreIsNegative_ShouldThrowArgumentException(int homeScore, int awayScore)
    {
        // Arrange
        var matchDate = DateTime.UtcNow;
        var homeTeamId = Guid.NewGuid();
        var awayTeamId = Guid.NewGuid();

        // Act
        Action act = () => new MatchResult(matchDate, homeTeamId, awayTeamId, homeScore, awayScore);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Away score cannot be negative*");
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 5)]
    [InlineData(10, 0)]
    public void Constructor_WhenScoresAreZeroOrPositive_ShouldCreateMatchResult(int homeScore, int awayScore)
    {
        // Arrange
        var matchDate = DateTime.UtcNow;
        var homeTeamId = Guid.NewGuid();
        var awayTeamId = Guid.NewGuid();

        // Act
        var matchResult = new MatchResult(matchDate, homeTeamId, awayTeamId, homeScore, awayScore);

        // Assert
        matchResult.HomeScore.Should().Be(homeScore);
        matchResult.AwayScore.Should().Be(awayScore);
    }

    [Fact]
    public void Constructor_WhenValidHighScores_ShouldCreateMatchResult()
    {
        // Arrange
        var matchDate = DateTime.UtcNow;
        var homeTeamId = Guid.NewGuid();
        var awayTeamId = Guid.NewGuid();
        var homeScore = 9;
        var awayScore = 8;

        // Act
        var matchResult = new MatchResult(matchDate, homeTeamId, awayTeamId, homeScore, awayScore);

        // Assert
        matchResult.HomeScore.Should().Be(homeScore);
        matchResult.AwayScore.Should().Be(awayScore);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds()
    {
        // Arrange
        var matchDate = DateTime.UtcNow;
        var homeTeamId = Guid.NewGuid();
        var awayTeamId = Guid.NewGuid();

        // Act
        var match1 = new MatchResult(matchDate, homeTeamId, awayTeamId, 1, 0);
        var match2 = new MatchResult(matchDate, homeTeamId, awayTeamId, 2, 1);

        // Assert
        match1.Id.Should().NotBe(match2.Id);
    }

    [Fact]
    public void PrivateConstructor_ForEfCore_ShouldCreateMatchResultWithGeneratedId()
    {
        // Act - Use reflection to call private constructor (simulating EF Core behavior)
        var matchResult = (MatchResult?)Activator.CreateInstance(typeof(MatchResult), true);

        // Assert
        matchResult.Should().NotBeNull();
        matchResult!.Id.Should().NotBe(Guid.Empty); // Entity base class generates a new Guid
        matchResult.HomeScore.Should().Be(0);
        matchResult.AwayScore.Should().Be(0);
    }
}
