using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameWeeksController : ControllerBase
{
    private readonly IGameWeekService _gameWeekService;

    public GameWeeksController(IGameWeekService gameWeekService)
    {
        _gameWeekService = gameWeekService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameWeekDto>>> GetAll()
    {
        var result = await _gameWeekService.GetAllGameWeeksAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameWeekDto>> GetById(Guid id)
    {
        var result = await _gameWeekService.GetGameWeekByIdAsync(id);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<ActionResult<GameWeekDto>> GetActive()
    {
        var result = await _gameWeekService.GetActiveGameWeekAsync();
        if (result is null)
            return NotFound(new { message = "No active game week found" });
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<GameWeekDto>> Create(CreateGameWeekDto dto)
    {
        var result = await _gameWeekService.CreateGameWeekAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id}/activate")]
    public async Task<ActionResult<GameWeekDto>> Activate(Guid id)
    {
        var result = await _gameWeekService.ActivateGameWeekAsync(id);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult<GameWeekDto>> Complete(Guid id)
    {
        var result = await _gameWeekService.CompleteGameWeekAsync(id);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _gameWeekService.DeleteGameWeekAsync(id);
        if (!success)
            return NotFound();
        return NoContent();
    }
}
