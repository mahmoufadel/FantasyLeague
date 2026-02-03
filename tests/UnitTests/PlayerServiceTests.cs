using System.Threading;
using Application.DTOs;
using Application.Services;
using Application.Interfaces;
using Domain.Entities;
using Moq;
using Xunit;
using FluentAssertions;

namespace UnitTests;

public class PlayerServiceTests
{
    [Theory, AutoMockData]
    public async Task GetAllPlayers_ReturnsMappedDtos(Mock<IPlayerRepository> mockRepo, List<Player> players)
    {
        // Arrange        
        mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(players);

        var service = new PlayerService(mockRepo.Object);

        // Act
        var result = await service.GetAllPlayersAsync();

        // Assert
        result.Should().HaveCount(players.Count);
        result.Should().Contain(p => p.Name == players.First().Name);
    }

    [Theory, AutoMockData]
    public async Task GetPlayerById_ReturnsDto_WhenFound(Mock<IPlayerRepository> mockRepo)
    {
        var player = new Player("Player1", "Forward", "ClubA", 10m);
        mockRepo.Setup(r => r.GetByIdAsync(player.Id, It.IsAny<CancellationToken>())).ReturnsAsync(player);

        var service = new PlayerService(mockRepo.Object);

        var result = await service.GetPlayerByIdAsync(player.Id);

        result.Should().NotBeNull();
        result!.Name.Should().Be(player.Name);
    }

    [Theory, AutoMockData]
    public async Task GetPlayersByPosition_ReturnsEmpty_WhenNoPlayers(Mock<IPlayerRepository> mockRepo)
    {
        mockRepo.Setup(r => r.GetByPositionAsync("Goalkeeper", It.IsAny<CancellationToken>())).ReturnsAsync(new List<Player>());

        var service = new PlayerService(mockRepo.Object);

        var result = await service.GetPlayersByPositionAsync("Goalkeeper");

        result.Should().BeEmpty();
    }

    [Theory, AutoMockData]
    public async Task CreatePlayer_AddsPlayer_AndReturnsDto(Mock<IPlayerRepository> mockRepo)
    {
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Player>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var service = new PlayerService(mockRepo.Object);
        var dto = new CreatePlayerDto("NewPlayer", "Defender", "ClubC", 6m);

        var result = await service.CreatePlayerAsync(dto);

        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
    }

    [Theory, AutoMockData]
    public async Task CreatePlayer_Throws_WhenPriceInvalid(Mock<IPlayerRepository> mockRepo)
    {
        var service = new PlayerService(mockRepo.Object);
        var dto = new CreatePlayerDto("BadPlayer", "Defender", "ClubC", 0m);

        await service.Invoking(s => s.CreatePlayerAsync(dto)).Should().ThrowAsync<ArgumentException>();
    }

    [Theory, AutoMockData]
    public async Task UpdatePlayerStats_ReturnsNull_WhenPlayerNotFound(Mock<IPlayerRepository> mockRepo)
    {
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Player?)null);

        var service = new PlayerService(mockRepo.Object);

        var result = await service.UpdatePlayerStatsAsync(Guid.NewGuid(), new UpdatePlayerStatsDto(1,1,0));

        result.Should().BeNull();
    }

    [Theory, AutoMockData]
    public async Task DeletePlayer_ReturnsFalse_WhenNotFound(Mock<IPlayerRepository> mockRepo)
    {
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Player?)null);

        var service = new PlayerService(mockRepo.Object);

        var result = await service.DeletePlayerAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }
}
