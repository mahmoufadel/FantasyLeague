using Domain.Entities;
using FluentAssertions;

namespace UnitTests.Domain.Entities;

public class TransferTests
{
    // ── Constructor — valid ────────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(5_000_000)]
    [InlineData(100_000_000)]
    public void Constructor_WhenFeeIsZeroOrPositive_ShouldCreateTransfer(decimal fee)
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var fromTeamId = Guid.NewGuid();
        var toTeamId = Guid.NewGuid();
        var date = DateTime.UtcNow;

        // Act
        var transfer = new Transfer(playerId, fromTeamId, toTeamId, date, fee);

        // Assert
        transfer.Should().NotBeNull();
        transfer.Id.Should().NotBeEmpty();
        transfer.PlayerId.Should().Be(playerId);
        transfer.FromTeamId.Should().Be(fromTeamId);
        transfer.ToTeamId.Should().Be(toTeamId);
        transfer.TransferDate.Should().Be(date);
        transfer.Fee.Should().Be(fee);
    }

    // ── Constructor — invalid PlayerId ─────────────────────────────────────

    [Fact]
    public void Constructor_WhenPlayerIdIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        var fromTeamId = Guid.NewGuid();
        var toTeamId = Guid.NewGuid();

        // Act
        Action act = () => new Transfer(Guid.Empty, fromTeamId, toTeamId, DateTime.UtcNow, 0m);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*PlayerId cannot be empty*");
    }

    // ── Constructor — invalid FromTeamId ───────────────────────────────────

    [Fact]
    public void Constructor_WhenFromTeamIdIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var toTeamId = Guid.NewGuid();

        // Act
        Action act = () => new Transfer(playerId, Guid.Empty, toTeamId, DateTime.UtcNow, 0m);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*FromTeamId cannot be empty*");
    }

    // ── Constructor — invalid ToTeamId ─────────────────────────────────────

    [Fact]
    public void Constructor_WhenToTeamIdIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var fromTeamId = Guid.NewGuid();

        // Act
        Action act = () => new Transfer(playerId, fromTeamId, Guid.Empty, DateTime.UtcNow, 0m);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*ToTeamId cannot be empty*");
    }

    // ── Constructor — same team ────────────────────────────────────────────

    [Fact]
    public void Constructor_WhenFromTeamIdEqualsToTeamId_ShouldThrowArgumentException()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var sameTeamId = Guid.NewGuid();

        // Act
        Action act = () => new Transfer(playerId, sameTeamId, sameTeamId, DateTime.UtcNow, 0m);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*FromTeamId and ToTeamId cannot be the same*");
    }

    // ── Constructor — negative fee ─────────────────────────────────────────

    [Theory]
    [InlineData(-1)]
    [InlineData(-1_000_000)]
    [InlineData(-0.01)]
    public void Constructor_WhenFeeIsNegative_ShouldThrowArgumentException(decimal fee)
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var fromTeamId = Guid.NewGuid();
        var toTeamId = Guid.NewGuid();

        // Act
        Action act = () => new Transfer(playerId, fromTeamId, toTeamId, DateTime.UtcNow, fee);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Fee cannot be negative*");
    }

    // ── Constructor — unique IDs ───────────────────────────────────────────

    [Fact]
    public void Constructor_WhenCalledTwice_ShouldGenerateUniqueIds()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var fromTeamId = Guid.NewGuid();
        var toTeamId = Guid.NewGuid();
        var date = DateTime.UtcNow;

        // Act
        var t1 = new Transfer(playerId, fromTeamId, toTeamId, date, 0m);
        var t2 = new Transfer(playerId, fromTeamId, toTeamId, date, 0m);

        // Assert
        t1.Id.Should().NotBe(t2.Id);
    }

    // ── EF Core private constructor ────────────────────────────────────────

    [Fact]
    public void PrivateConstructor_WhenInvokedByReflection_ShouldCreateTransferWithGeneratedId()
    {
        // Act
        var transfer = (Transfer?)Activator.CreateInstance(typeof(Transfer), nonPublic: true);

        // Assert
        transfer.Should().NotBeNull();
        transfer!.Id.Should().NotBe(Guid.Empty);
    }
}
