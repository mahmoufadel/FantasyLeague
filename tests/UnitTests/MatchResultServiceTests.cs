using System.Threading;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;
using Xunit;
using FluentAssertions;

namespace UnitTests;

public class MatchResultServiceTests
{
    [Theory, AutoMockData]
    public async Task GetAllMatchResults_ReturnsMappedDtos(Mock<IMatchResultRepository> mockRepo, List<MatchResult> matches)
    {
        // Arrange
        mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(matches);

        var service = new MatchResultService(mockRepo.Object);

        // Act
        var result = await service.GetAllMatchResultsAsync();

        // Assert
        result.Should().HaveCount(matches.Count);
        result.Should().Contain(m => m.MatchId == matches.First().Id);
    }

    [Theory, AutoMockData]
    public async Task GetMatchResultById_ReturnsDto_WhenFound(Mock<IMatchResultRepository> mockRepo)
    {
        // Arrange
        var match = new MatchResult(DateTime.UtcNow, Guid.NewGuid(), Guid.NewGuid(), 2, 1);
        mockRepo.Setup(r => r.GetByIdAsync(match.Id, It.IsAny<CancellationToken>())).ReturnsAsync(match);

        var service = new MatchResultService(mockRepo.Object);

        // Act
        var result = await service.GetMatchResultByIdAsync(match.Id);

        // Assert
        result.Should().NotBeNull();
        result!.HomeScore.Should().Be(match.HomeScore);
        result.AwayScore.Should().Be(match.AwayScore);
    }

    [Theory, AutoMockData]
    public async Task GetMatchResultById_ReturnsNull_WhenNotFound(Mock<IMatchResultRepository> mockRepo, Guid nonExistentId)
    {
        // Arrange
        mockRepo.Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>())).ReturnsAsync((MatchResult?)null);

        var service = new MatchResultService(mockRepo.Object);

        // Act
        var result = await service.GetMatchResultByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Theory, AutoMockData]
    public async Task CreateMatchResult_AddsMatch_AndReturnsDto(Mock<IMatchResultRepository> mockRepo)
    {
        // Arrange
        mockRepo.Setup(r => r.AddAsync(It.IsAny<MatchResult>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var service = new MatchResultService(mockRepo.Object);
        var dto = new CreateMatchResultDto(Guid.NewGuid(), Guid.NewGuid(), 3, 2, DateTime.UtcNow);

        // Act
        var result = await service.CreateMatchResultAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.HomeTeamId.Should().Be(dto.HomeTeamId);
        result.AwayTeamId.Should().Be(dto.AwayTeamId);
        result.HomeScore.Should().Be(dto.HomeScore);
        result.AwayScore.Should().Be(dto.AwayScore);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<MatchResult>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory, AutoMockData]
    public async Task CreateMatchResult_UsesCurrentDate_WhenMatchDateNotProvided(Mock<IMatchResultRepository> mockRepo)
    {
        // Arrange
        mockRepo.Setup(r => r.AddAsync(It.IsAny<MatchResult>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var service = new MatchResultService(mockRepo.Object);
        var dto = new CreateMatchResultDto(Guid.NewGuid(), Guid.NewGuid(), 1, 0, null);

        // Act
        var result = await service.CreateMatchResultAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.MatchDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory, AutoMockData]
    public async Task CreateMatchResult_Throws_WhenHomeTeamIdIsEmpty(Mock<IMatchResultRepository> mockRepo)
    {
        // Arrange
        var service = new MatchResultService(mockRepo.Object);
        var dto = new CreateMatchResultDto(Guid.Empty, Guid.NewGuid(), 1, 0, DateTime.UtcNow);

        // Act & Assert
        await service.Invoking(s => s.CreateMatchResultAsync(dto))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*HomeTeamId cannot be empty*");
    }

    [Theory, AutoMockData]
    public async Task CreateMatchResult_Throws_WhenAwayTeamIdIsEmpty(Mock<IMatchResultRepository> mockRepo)
    {
        // Arrange
        var service = new MatchResultService(mockRepo.Object);
        var dto = new CreateMatchResultDto(Guid.NewGuid(), Guid.Empty, 1, 0, DateTime.UtcNow);

        // Act & Assert
        await service.Invoking(s => s.CreateMatchResultAsync(dto))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*AwayTeamId cannot be empty*");
    }

    [Theory, AutoMockData]
    public async Task CreateMatchResult_Throws_WhenTeamsAreSame(Mock<IMatchResultRepository> mockRepo)
    {
        // Arrange
        var service = new MatchResultService(mockRepo.Object);
        var sameTeamId = Guid.NewGuid();
        var dto = new CreateMatchResultDto(sameTeamId, sameTeamId, 1, 0, DateTime.UtcNow);

        // Act & Assert
        await service.Invoking(s => s.CreateMatchResultAsync(dto))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Home team and away team cannot be the same*");
    }

    [Theory, AutoMockData]
    public async Task CreateMatchResult_Throws_WhenHomeScoreIsNegative(Mock<IMatchResultRepository> mockRepo)
    {
        // Arrange
        var service = new MatchResultService(mockRepo.Object);
        var dto = new CreateMatchResultDto(Guid.NewGuid(), Guid.NewGuid(), -1, 0, DateTime.UtcNow);

        // Act & Assert
        await service.Invoking(s => s.CreateMatchResultAsync(dto))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Home score cannot be negative*");
    }

    [Theory, AutoMockData]
    public async Task CreateMatchResult_Throws_WhenAwayScoreIsNegative(Mock<IMatchResultRepository> mockRepo)
    {
        // Arrange
        var service = new MatchResultService(mockRepo.Object);
        var dto = new CreateMatchResultDto(Guid.NewGuid(), Guid.NewGuid(), 0, -1, DateTime.UtcNow);

        // Act & Assert
        await service.Invoking(s => s.CreateMatchResultAsync(dto))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Away score cannot be negative*");
    }
}
