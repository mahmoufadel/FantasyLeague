using Application.DTOs;
using Application.Interfaces;
using WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchResultsController : ControllerBase
{
    private readonly IMatchResultService _matchResultService;
    private readonly DapprClient _dapprClient;
    private readonly ILogger<MatchResultsController> _logger;

    public MatchResultsController(IMatchResultService matchResultService, DapprClient daprClient, ILogger<MatchResultsController> logger)
    {
        _matchResultService = matchResultService;
        _dapprClient = daprClient;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMatchResults(CancellationToken cancellationToken)
    {
        var results = await _matchResultService.GetAllMatchResultsAsync(cancellationToken);
        return Ok(results);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMatchResult(Guid id, CancellationToken cancellationToken)
    {
        var result = await _matchResultService.GetMatchResultByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMatchResult([FromBody] CreateMatchResultDto dto, CancellationToken cancellationToken)
    {
        var result = await _matchResultService.CreateMatchResultAsync(dto, cancellationToken);

        // Publish the match result to a Dapr pub/sub component named "matchpubsub" and topic "matchresults"
        try
        {
            await _dapprClient.PublishEventAsync("matchpubsub", "matchresults", result, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            // If Dapr isn't running the publish will fail; we still return the created result
            _logger.LogWarning(ex, "Dapr publish failed for match result {MatchId}", result.MatchId);
        }

        return CreatedAtAction(nameof(GetMatchResult), new { id = result.MatchId }, result);
    }
}

