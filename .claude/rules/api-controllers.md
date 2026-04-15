---
description: ASP.NET Core controller conventions for the WebApi layer
globs: ["src/WebApi/Controllers/**/*.cs"]
alwaysApply: false
---

# API Controller Rules

Controllers are thin HTTP adapters. They receive a request, delegate to a service, and return an HTTP response. No business logic lives here.

## Structure

```csharp
[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayersController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlayerDto>> GetById(Guid id)
    {
        var result = await _playerService.GetPlayerByIdAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }
}
```

## HTTP Status Code Conventions

| Scenario | Status code | Return |
|----------|-------------|--------|
| Successful read | 200 OK | `Ok(dto)` |
| Resource created | 201 Created | `CreatedAtAction(nameof(GetById), new { id }, dto)` |
| Resource not found | 404 Not Found | `NotFound()` |
| Invalid input / business rule violation | 400 Bad Request | `BadRequest(message)` |
| Successful delete | 204 No Content | `NoContent()` |

## Exception Handling

Catch `InvalidOperationException` at the controller boundary and return `BadRequest(ex.Message)`. Let all other exceptions propagate (the framework handles 500s).

```csharp
try
{
    var result = await _teamService.AddPlayerToTeamAsync(dto);
    return Ok(result);
}
catch (InvalidOperationException ex)
{
    return BadRequest(ex.Value);
}
```

## Forbidden in Controllers

- No `DbContext` or repository references — only service interfaces
- No business logic, calculations, or validation beyond model binding
- No domain entity types in return values — always return DTOs
- No static methods

## Route Conventions

- Base route: `api/[controller]` (lowercase, plural noun)
- Sub-resources: `[HttpPost("add-player")]`, `[HttpDelete("{teamId}/players/{playerId}")]`
- ID parameters are always `Guid`
