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

    public MatchResultsController(IMatchResultService matchResultService, DapprClient daprClient)
    {
        _matchResultService = matchResultService;
        _dapprClient = daprClient;
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
            Console.WriteLine($"Dapr publish failed: {ex.Message}");
        }

        return CreatedAtAction(nameof(GetMatchResult), new { id = result.MatchId }, result);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetMatchResult(Guid id)
    {
        // This example doesn't persist match results; return NotFound
        return NotFound();
    }
}
