using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeamDto>>> GetAll()
    {
        var teams = await _teamService.GetAllTeamsAsync();
        return Ok(teams);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TeamDto>> GetById(Guid id)
    {
        var team = await _teamService.GetTeamByIdAsync(id);
        if (team == null)
            return NotFound();

        return Ok(team);
    }

    [HttpPost]
    public async Task<ActionResult<TeamDto>> Create(CreateTeamDto dto)
    {
        var team = await _teamService.CreateTeamAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = team.Id }, team);
    }

    [HttpPost("add-player")]
    public async Task<ActionResult<TeamDto>> AddPlayer(AddPlayerToTeamDto dto)
    {
        try
        {
            var team = await _teamService.AddPlayerToTeamAsync(dto);
            if (team == null)
                return NotFound("Team not found");

            return Ok(team);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{teamId}/players/{playerId}")]
    public async Task<ActionResult<TeamDto>> RemovePlayer(Guid teamId, Guid playerId)
    {
        try
        {
            var team = await _teamService.RemovePlayerFromTeamAsync(teamId, playerId);
            if (team == null)
                return NotFound("Team not found");

            return Ok(team);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _teamService.DeleteTeamAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
