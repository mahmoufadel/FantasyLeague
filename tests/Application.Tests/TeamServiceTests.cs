using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class TeamServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ITeamRepository> _teamRepositoryMock;
        private readonly Mock<IPlayerRepository> _playerRepositoryMock;
        private readonly TeamService _sut;

        public TeamServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _teamRepositoryMock = _fixture.Freeze<Mock<ITeamRepository>>();
            _playerRepositoryMock = _fixture.Freeze<Mock<IPlayerRepository>>();
            _sut = new TeamService(_teamRepositoryMock.Object, _playerRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllTeamsAsync_WhenCalled_ShouldReturnTeamDtos()
        {
            // Arrange
            var team = new Team("Team A", "Manager A");
            _teamRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Team> { team });

            _playerRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken ct) => null as Player);

            // Act
            var result = await _sut.GetAllTeamsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Should().ContainSingle(t => t.Name == "Team A" && t.ManagerName == "Manager A");
        }

        [Fact]
        public async Task CreateTeamAsync_WhenCalled_ShouldAddTeamAndReturnDto()
        {
            // Arrange
            var dto = new CreateTeamDto("New Team", "New Manager");

            // Act
            var result = await _sut.CreateTeamAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("New Team");
            result.ManagerName.Should().Be("New Manager");
            _teamRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddPlayerToTeamAsync_WhenTeamNotFound_ShouldReturnNull()
        {
            // Arrange
            var dto = new AddPlayerToTeamDto(Guid.NewGuid(), Guid.NewGuid());
            _teamRepositoryMock.Setup(r => r.GetByIdAsync(dto.TeamId, It.IsAny<CancellationToken>())).ReturnsAsync((Team)null);

            // Act
            var result = await _sut.AddPlayerToTeamAsync(dto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddPlayerToTeamAsync_WhenPlayerNotFound_ShouldThrow()
        {
            // Arrange
            var team = new Team("Team A", "Manager A");
            var dto = new AddPlayerToTeamDto(Guid.NewGuid(), Guid.NewGuid());
            _teamRepositoryMock.Setup(r => r.GetByIdAsync(dto.TeamId, It.IsAny<CancellationToken>())).ReturnsAsync(team);
            _playerRepositoryMock.Setup(r => r.GetByIdAsync(dto.PlayerId, It.IsAny<CancellationToken>())).ReturnsAsync((Player)null);

            // Act / Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddPlayerToTeamAsync(dto));
        }

        [Fact]
        public async Task RemovePlayerFromTeamAsync_WhenTeamNotFound_ShouldReturnNull()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var playerId = Guid.NewGuid();
            _teamRepositoryMock.Setup(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>())).ReturnsAsync((Team)null);

            // Act
            var result = await _sut.RemovePlayerFromTeamAsync(teamId, playerId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteTeamAsync_WhenTeamNotFound_ShouldReturnFalse()
        {
            // Arrange
            var id = Guid.NewGuid();
            _teamRepositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Team)null);

            // Act
            var result = await _sut.DeleteTeamAsync(id);

            // Assert
            result.Should().BeFalse();
        }
    }
}
