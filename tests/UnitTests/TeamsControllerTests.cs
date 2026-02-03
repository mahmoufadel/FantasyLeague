using Microsoft.AspNetCore.Mvc;
using Moq;
using Application.DTOs;
using WebApi.Controllers;
using Xunit;
using FluentAssertions;
using Application.Interfaces;

namespace UnitTests;

public class TeamsControllerTests
{
    [Theory, AutoMockData]
    public async Task GetAll_ReturnsOk(Mock<ITeamService> mockService)
    {
        mockService.Setup(s => s.GetAllTeamsAsync(It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new List<TeamDto>());

        var controller = new TeamsController(mockService.Object);

        var result = await controller.GetAll();

        var actionResult = Assert.IsType<ActionResult<IEnumerable<TeamDto>>>(result);
        actionResult.Result.Should().BeOfType<OkObjectResult>();
    }

    [Theory, AutoMockData]
    public async Task GetById_ReturnsNotFound_WhenNull(Mock<ITeamService> mockService)
    {
        mockService.Setup(s => s.GetTeamByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync((TeamDto?)null);

        var controller = new TeamsController(mockService.Object);

        var result = await controller.GetById(Guid.NewGuid());

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Theory, AutoMockData]
    public async Task AddPlayer_ReturnsBadRequest_WhenPlayerMissing(Mock<ITeamService> mockService)
    {
        mockService.Setup(s => s.AddPlayerToTeamAsync(It.IsAny<AddPlayerToTeamDto>(), It.IsAny<System.Threading.CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Player not found"));

        var controller = new TeamsController(mockService.Object);

        var result = await controller.AddPlayer(new AddPlayerToTeamDto(Guid.NewGuid(), Guid.NewGuid()));

        var actionResult = Assert.IsType<ActionResult<TeamDto>>(result);
        actionResult.Result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be("Player not found");
    }

    [Theory, AutoMockData]
    public async Task Delete_ReturnsNotFound_WhenTeamMissing(Mock<ITeamService> mockService)
    {
        mockService.Setup(s => s.DeleteTeamAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(false);

        var controller = new TeamsController(mockService.Object);

        var result = await controller.Delete(Guid.NewGuid());

        result.Should().BeOfType<NotFoundResult>();
    }
}
