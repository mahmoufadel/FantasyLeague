using Domain.Entities;
using FluentAssertions;

namespace UnitTests.Domain.Entities;

public class GameWeekTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(38)]
    public void Constructor_WhenValidData_ShouldCreateGameWeek(int weekNumber)
    {
        // Arrange
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(7);

        // Act
        var gameWeek = new GameWeek(weekNumber, startDate, endDate);

        // Assert
        gameWeek.Should().NotBeNull();
        gameWeek.Id.Should().NotBeEmpty();
        gameWeek.WeekNumber.Should().Be(weekNumber);
        gameWeek.StartDate.Should().Be(startDate);
        gameWeek.EndDate.Should().Be(endDate);
        gameWeek.IsActive.Should().BeFalse();
        gameWeek.IsCompleted.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WhenWeekNumberIsZeroOrNegative_ShouldThrowArgumentException(int weekNumber)
    {
        // Arrange
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(7);

        // Act
        Action act = () => new GameWeek(weekNumber, startDate, endDate);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Week number must be greater than zero*");
    }

    [Fact]
    public void Constructor_WhenEndDateBeforeStartDate_ShouldThrowArgumentException()
    {
        // Arrange
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(-1);

        // Act
        Action act = () => new GameWeek(1, startDate, endDate);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Start date must be before end date*");
    }

    [Fact]
    public void Constructor_WhenSameStartAndEndDate_ShouldThrowArgumentException()
    {
        // Arrange
        var date = DateTime.UtcNow;

        // Act
        Action act = () => new GameWeek(1, date, date);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Start date must be before end date*");
    }

    [Fact]
    public void Activate_WhenNotCompleted_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var gameWeek = new GameWeek(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        gameWeek.IsActive.Should().BeFalse();

        // Act
        gameWeek.Activate();

        // Assert
        gameWeek.IsActive.Should().BeTrue();
        gameWeek.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ShouldRemainActive()
    {
        // Arrange
        var gameWeek = new GameWeek(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        gameWeek.Activate();
        gameWeek.IsActive.Should().BeTrue();

        // Act
        gameWeek.Activate();

        // Assert
        gameWeek.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Activate_WhenCompleted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var gameWeek = new GameWeek(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        gameWeek.Activate();
        gameWeek.Complete();

        // Act
        Action act = () => gameWeek.Activate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot activate a completed game week*");
    }

    [Fact]
    public void Complete_WhenActive_ShouldSetIsCompletedToTrueAndIsActiveToFalse()
    {
        // Arrange
        var gameWeek = new GameWeek(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        gameWeek.Activate();
        gameWeek.IsActive.Should().BeTrue();

        // Act
        gameWeek.Complete();

        // Assert
        gameWeek.IsCompleted.Should().BeTrue();
        gameWeek.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Complete_WhenNotActive_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var gameWeek = new GameWeek(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));

        // Act
        Action act = () => gameWeek.Complete();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot complete an inactive game week*");
    }

    [Fact]
    public void Complete_WhenAlreadyCompleted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var gameWeek = new GameWeek(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        gameWeek.Activate();
        gameWeek.Complete();

        // Act
        Action act = () => gameWeek.Complete();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot complete an inactive game week*");
    }

    [Fact]
    public void PrivateConstructor_ForEfCore_ShouldCreateGameWeekWithDefaultValues()
    {
        // Act - Use reflection to call private constructor
        var gameWeek = (GameWeek?)Activator.CreateInstance(typeof(GameWeek), true);

        // Assert
        gameWeek.Should().NotBeNull();
        gameWeek!.Id.Should().NotBe(Guid.Empty);
        gameWeek.WeekNumber.Should().Be(0);
        gameWeek.IsActive.Should().BeFalse();
        gameWeek.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds()
    {
        // Arrange
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(7);

        // Act
        var gameWeek1 = new GameWeek(1, startDate, endDate);
        var gameWeek2 = new GameWeek(2, startDate, endDate);

        // Assert
        gameWeek1.Id.Should().NotBe(gameWeek2.Id);
    }
}
