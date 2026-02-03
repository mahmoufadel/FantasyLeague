using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayersController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAll()
    {
        var players = await _playerService.GetAllPlayersAsync();
        return Ok(players);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlayerDto>> GetById(Guid id)
    {
        var player = await _playerService.GetPlayerByIdAsync(id);
        if (player == null)
            return NotFound();

        return Ok(player);
    }

    [HttpGet("position/{position}")]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetByPosition(string position)
    {
        var players = await _playerService.GetPlayersByPositionAsync(position);
        return Ok(players);
    }

    [HttpPost]
    public async Task<ActionResult<PlayerDto>> Create(CreatePlayerDto dto)
    {
        var player = await _playerService.CreatePlayerAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = player.Id }, player);
    }

    [HttpPut("{id}/stats")]
    public async Task<ActionResult<PlayerDto>> UpdateStats(Guid id, UpdatePlayerStatsDto dto)
    {
        var player = await _playerService.UpdatePlayerStatsAsync(id, dto);
        if (player == null)
            return NotFound();

        return Ok(player);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _playerService.DeletePlayerAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
