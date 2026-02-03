using Microsoft.AspNetCore.Mvc;
using Moq;
using Application.Services;
using Application.DTOs;
using WebApi.Controllers;
using Xunit;
using FluentAssertions;
using Application.Interfaces;

namespace UnitTests;

public class PlayersControllerTests
{
    [Theory, AutoMockData]
    public async Task GetAll_ReturnsOkWithPlayers(Mock<IPlayerService> mockService)
    {
        mockService.Setup(s => s.GetAllPlayersAsync(It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new List<PlayerDto> { new PlayerDto(Guid.NewGuid(), "P1", "Forward", "C1", 10m, 0, 0, 0, 0) });

        var controller = new PlayersController(mockService.Object);

        var result = await controller.GetAll();

        var actionResult = Assert.IsType<ActionResult<IEnumerable<PlayerDto>>>(result);
        actionResult.Result.Should().BeOfType<OkObjectResult>();
    }

    [Theory, AutoMockData]
    public async Task GetById_ReturnsNotFound_WhenNull(Mock<IPlayerService> mockService)
    {
        mockService.Setup(s => s.GetPlayerByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync((PlayerDto?)null);

        var controller = new PlayersController(mockService.Object);

        var result = await controller.GetById(Guid.NewGuid());

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Theory, AutoMockData]
    public async Task Create_ReturnsCreatedAtAction(Mock<IPlayerService> mockService)
    {
        var created = new PlayerDto(Guid.NewGuid(), "P2", "Defender", "C2", 7m, 0, 0, 0, 0);
        mockService.Setup(s => s.CreatePlayerAsync(It.IsAny<CreatePlayerDto>(), It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(created);

        var controller = new PlayersController(mockService.Object);

        var result = await controller.Create(new CreatePlayerDto("P2", "Defender", "C2", 7m));

        var actionResult = Assert.IsType<ActionResult<PlayerDto>>(result);
        actionResult.Result.Should().BeOfType<CreatedAtActionResult>().Which.Value.Should().Be(created);
    }

    [Theory, AutoMockData]
    public async Task UpdateStats_ReturnsNotFound_WhenPlayerMissing(Mock<IPlayerService> mockService)
    {
        mockService.Setup(s => s.UpdatePlayerStatsAsync(It.IsAny<Guid>(), It.IsAny<UpdatePlayerStatsDto>(), It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync((PlayerDto?)null);

        var controller = new PlayersController(mockService.Object);

        var result = await controller.UpdateStats(Guid.NewGuid(), new UpdatePlayerStatsDto(1,1,0));

        result.Result.Should().BeOfType<NotFoundResult>();
    }
}
