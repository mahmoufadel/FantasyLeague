using McpServer.Models;
using McpServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace McpServer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly WorldCupStore _store;

    public MatchesController(WorldCupStore store)
    {
        _store = store;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var matches = _store.GetAllMatches();
        return Ok(new { success = true, count = matches.Count, data = matches });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var match = _store.GetMatch(id);
        if (match == null)
            return NotFound(new { success = false, error = "Match not found" });

        return Ok(new { success = true, data = match });
    }

    [HttpGet("stage/{stage}")]
    public IActionResult GetByStage(string stage)
    {
        if (!Enum.TryParse<TournamentStage>(stage, true, out var tournamentStage))
        {
            return BadRequest(new
            {
                success = false,
                error = $"Invalid stage. Valid values: {string.Join(", ", Enum.GetNames<TournamentStage>())}"
            });
        }

        var matches = _store.GetMatchesByStage(tournamentStage);
        return Ok(new { success = true, count = matches.Count, data = matches });
    }

    [HttpGet("team/{teamName}")]
    public IActionResult GetByTeam(string teamName)
    {
        var matches = _store.GetMatchesByTeam(teamName);
        return Ok(new { success = true, count = matches.Count, data = matches });
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateMatchRequest request)
    {
        try
        {
            if (!Enum.TryParse<TournamentStage>(request.Stage, true, out var stage))
            {
                return BadRequest(new
                {
                    success = false,
                    error = $"Invalid stage. Valid values: {string.Join(", ", Enum.GetNames<TournamentStage>())}"
                });
            }

            var match = _store.CreateMatch(
                request.HomeTeam,
                request.AwayTeam,
                DateTime.Parse(request.MatchDate),
                stage,
                request.Venue,
                request.Group
            );

            return CreatedAtAction(nameof(GetById), new { id = match.Id },
                new { success = true, message = "Match created successfully", data = match });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpPut("{id}/score")]
    public IActionResult UpdateScore(string id, [FromBody] UpdateScoreRequest request)
    {
        var match = _store.UpdateMatchScore(id, request.HomeScore, request.AwayScore);
        if (match == null)
            return NotFound(new { success = false, error = "Match not found" });

        return Ok(new { success = true, message = "Score updated", data = match });
    }

    [HttpPost("{id}/start")]
    public IActionResult StartMatch(string id)
    {
        var match = _store.StartMatch(id);
        if (match == null)
            return NotFound(new { success = false, error = "Match not found" });

        return Ok(new { success = true, message = "Match started", data = match });
    }

    [HttpGet("{id}/messages")]
    public IActionResult GetMessages(string id)
    {
        var match = _store.GetMatch(id);
        if (match == null)
            return NotFound(new { success = false, error = "Match not found" });

        var messages = _store.GetMessagesForMatch(id);
        return Ok(new { success = true, count = messages.Count, data = messages });
    }
}

public class CreateMatchRequest
{
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public string MatchDate { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;
    public string? Group { get; set; }
}

public class UpdateScoreRequest
{
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
}
