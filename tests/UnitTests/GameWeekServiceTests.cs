using System.Threading;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;
using Xunit;
using FluentAssertions;

namespace UnitTests;

public class GameWeekServiceTests
{
    [Theory, AutoMockData]
    public async Task GetAllGameWeeks_ReturnsMappedDtos(Mock<IGameWeekRepository> mockRepo)
    {
        // Arrange
        var gameWeeks = new List<GameWeek>
        {
            new GameWeek(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(7)),
            new GameWeek(2, DateTime.UtcNow.AddDays(7), DateTime.UtcNow.AddDays(14))
        };
        mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(gameWeeks);

        var service = new GameWeekService(mockRepo.Object);

        // Act
        var result = await service.GetAllGameWeeksAsync();

        // Assert
        result.Should().HaveCount(gameWeeks.Count);
    }

    [Theory, AutoMockData]
    public async Task GetGameWeekById_ReturnsDto_WhenFound(Mock<IGameWeekRepository> mockRepo)
    {
        // Arrange
        var gameWeek = new GameWeek(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        mockRepo.Setup(r => r.GetByIdAsync(gameWeek.Id, It.IsAny<CancellationToken>())).ReturnsAsync(gameWeek);

        var service = new GameWeekService(mockRepo.Object);

        // Act
        var result = await service.GetGameWeekByIdAsync(gameWeek.Id);

        // Assert
        result.Should().NotBeNull();
        result!.WeekNumber.Should().Be(gameWeek.WeekNumber);
    }

    [Theory, AutoMockData]
    public async Task GetGameWeekById_ReturnsNull_WhenNotFound(Mock<IGameWeekRepository> mockRepo, Guid nonExistentId)
    {
        // Arrange
        mockRepo.Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>())).ReturnsAsync((GameWeek?)null);

        var service = new GameWeekService(mockRepo.Object);

        // Act
        var result = await service.GetGameWeekByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Theory, AutoMockData]
    public async Task GetActiveGameWeek_ReturnsDto_WhenFound(Mock<IGameWeekRepository> mockRepo)
    {
        // Arrange
        var gameWeek = new GameWeek(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        gameWeek.Activate();
        mockRepo.Setup(r => r.GetActiveGameWeekAsync(It.IsAny<CancellationToken>())).ReturnsAsync(gameWeek);

        var service = new GameWeekService(mockRepo.Object);

        // Act
        var result = await service.GetActiveGameWeekAsync();

        // Assert
        result.Should().NotBeNull();
        result!.IsActive.Should().BeTrue();
    }

    [Theory, AutoMockData]
    public async Task GetActiveGameWeek_ReturnsNull_WhenNoneActive(Mock<IGameWeekRepository> mockRepo)
    {
        // Arrange
        mockRepo.Setup(r => r.GetActiveGameWeekAsync(It.IsAny<CancellationToken>())).ReturnsAsync((GameWeek?)null);

        var service = new GameWeekService(mockRepo.Object);

        // Act
        var result = await service.GetActiveGameWeekAsync();

        // Assert
        result.Should().BeNull();
    }

    [Theory, AutoMockData]
    public async Task CreateGameWeek_AddsGameWeek_AndReturnsDto(Mock<IGameWeekRepository> mockRepo)
    {
        // Arrange
        mockRepo.Setup(r => r.AddAsync(It.IsAny<GameWeek>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var service = new GameWeekService(mockRepo.Object);
        var dto = new CreateGameWeekDto(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));

        // Act
        var result = await service.CreateGameWeekAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.WeekNumber.Should().Be(dto.WeekNumber);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<GameWeek>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory, AutoMockData]
    public async Task ActivateGameWeek_ActivatesAndReturnsDto_WhenFound(Mock<IGameWeekRepository> mockRepo)
    {
        // Arrange
        var gameWeek = new GameWeek(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        mockRepo.Setup(r => r.GetByIdAsync(gameWeek.Id, It.IsAny<CancellationToken>())).ReturnsAsync(gameWeek);
        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<GameWeek>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var service = new GameWeekService(mockRepo.Object);

        // Act
        var result = await service.ActivateGameWeekAsync(gameWeek.Id);

        // Assert
        result.Should().NotBeNull();
        result!.IsActive.Should().BeTrue();
        mockRepo.Verify(r => r.UpdateAsync(It.IsAny<GameWeek>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory, AutoMockData]
    public async Task ActivateGameWeek_ReturnsNull_WhenNotFound(Mock<IGameWeekRepository> mockRepo, Guid nonExistentId)
    {
        // Arrange
        mockRepo.Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>())).ReturnsAsync((GameWeek?)null);

        var service = new GameWeekService(mockRepo.Object);

        // Act
        var result = await service.ActivateGameWeekAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Theory, AutoMockData]
    public async Task CompleteGameWeek_CompletesAndReturnsDto_WhenFound(Mock<IGameWeekRepository> mockRepo)
    {
        // Arrange
        var gameWeek = new GameWeek(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        gameWeek.Activate();
        mockRepo.Setup(r => r.GetByIdAsync(gameWeek.Id, It.IsAny<CancellationToken>())).ReturnsAsync(gameWeek);
        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<GameWeek>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var service = new GameWeekService(mockRepo.Object);

        // Act
        var result = await service.CompleteGameWeekAsync(gameWeek.Id);

        // Assert
        result.Should().NotBeNull();
        result!.IsCompleted.Should().BeTrue();
        result.IsActive.Should().BeFalse();
        mockRepo.Verify(r => r.UpdateAsync(It.IsAny<GameWeek>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory, AutoMockData]
    public async Task CompleteGameWeek_ReturnsNull_WhenNotFound(Mock<IGameWeekRepository> mockRepo, Guid nonExistentId)
    {
        // Arrange
        mockRepo.Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>())).ReturnsAsync((GameWeek?)null);

        var service = new GameWeekService(mockRepo.Object);

        // Act
        var result = await service.CompleteGameWeekAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Theory, AutoMockData]
    public async Task DeleteGameWeek_ReturnsTrue_WhenFound(Mock<IGameWeekRepository> mockRepo)
    {
        // Arrange
        var gameWeek = new GameWeek(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        mockRepo.Setup(r => r.GetByIdAsync(gameWeek.Id, It.IsAny<CancellationToken>())).ReturnsAsync(gameWeek);
        mockRepo.Setup(r => r.DeleteAsync(gameWeek.Id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var service = new GameWeekService(mockRepo.Object);

        // Act
        var result = await service.DeleteGameWeekAsync(gameWeek.Id);

        // Assert
        result.Should().BeTrue();
        mockRepo.Verify(r => r.DeleteAsync(gameWeek.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory, AutoMockData]
    public async Task DeleteGameWeek_ReturnsFalse_WhenNotFound(Mock<IGameWeekRepository> mockRepo, Guid nonExistentId)
    {
        // Arrange
        mockRepo.Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>())).ReturnsAsync((GameWeek?)null);

        var service = new GameWeekService(mockRepo.Object);

        // Act
        var result = await service.DeleteGameWeekAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Theory, AutoMockData]
    public async Task CreateGameWeek_Throws_WhenWeekNumberInvalid(Mock<IGameWeekRepository> mockRepo)
    {
        // Arrange
        var service = new GameWeekService(mockRepo.Object);
        var dto = new CreateGameWeekDto(0, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));

        // Act & Assert
        await service.Invoking(s => s.CreateGameWeekAsync(dto))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Week number must be greater than zero*");
    }

    [Theory, AutoMockData]
    public async Task CreateGameWeek_Throws_WhenEndDateBeforeStartDate(Mock<IGameWeekRepository> mockRepo)
    {
        // Arrange
        var service = new GameWeekService(mockRepo.Object);
        var startDate = DateTime.UtcNow;
        var dto = new CreateGameWeekDto(1, startDate, startDate.AddDays(-1));

        // Act & Assert
        await service.Invoking(s => s.CreateGameWeekAsync(dto))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Start date must be before end date*");
    }
}
