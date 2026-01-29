using System.Threading;
using Application.DTOs;
using Application.Services;
using Application.Interfaces;
using Domain.Entities;
using Moq;
using Xunit;
using FluentAssertions;

namespace UnitTests;

public class TeamServiceTests
{
    [Theory, AutoMockData]
    public async Task CreateTeam_ReturnsTeamDto(string teamName, string managerName, Mock<ITeamRepository> mockTeamRepo, Mock<IPlayerRepository> mockPlayerRepo)
    {
        mockTeamRepo.Setup(r => r.AddAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var service = new TeamService(mockTeamRepo.Object, mockPlayerRepo.Object);

        var dto = new CreateTeamDto(teamName, managerName);

        var result = await service.CreateTeamAsync(dto);

        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.ManagerName.Should().Be(dto.ManagerName);
    }

    [Theory, AutoMockData]
    public async Task AddPlayerToTeam_Throws_WhenPlayerNotFound(string teamName, string managerName, Mock<ITeamRepository> mockTeamRepo, Mock<IPlayerRepository> mockPlayerRepo)
    {
        var team = new Team(teamName, managerName);
        mockTeamRepo.Setup(r => r.GetByIdAsync(team.Id, It.IsAny<CancellationToken>())).ReturnsAsync(team);
        mockPlayerRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Player?)null);

        var service = new TeamService(mockTeamRepo.Object, mockPlayerRepo.Object);

        await service.Invoking(s => s.AddPlayerToTeamAsync(new AddPlayerToTeamDto(team.Id, Guid.NewGuid()))).Should().ThrowAsync<InvalidOperationException>();
    }

    [Theory, AutoMockData]
    public async Task GetTeamById_ReturnsNull_WhenNotFound(Mock<ITeamRepository> mockTeamRepo, Mock<IPlayerRepository> mockPlayerRepo)
    {
        mockTeamRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Team?)null);

        var service = new TeamService(mockTeamRepo.Object, mockPlayerRepo.Object);

        var result = await service.GetTeamByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Theory, AutoMockData]
    public async Task RemovePlayerFromTeam_Throws_WhenPlayerMissing(string teamName, string managerName, Mock<ITeamRepository> mockTeamRepo, Mock<IPlayerRepository> mockPlayerRepo)
    {
        var team = new Team(teamName, managerName);
        mockTeamRepo.Setup(r => r.GetByIdAsync(team.Id, It.IsAny<CancellationToken>())).ReturnsAsync(team);
        mockPlayerRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Player?)null);

        var service = new TeamService(mockTeamRepo.Object, mockPlayerRepo.Object);

        await service.Invoking(s => s.RemovePlayerFromTeamAsync(team.Id, Guid.NewGuid())).Should().ThrowAsync<InvalidOperationException>();
    }
}
