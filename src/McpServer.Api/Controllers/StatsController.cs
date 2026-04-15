using McpServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace McpServer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly WorldCupStore _store;

    public StatsController(WorldCupStore store)
    {
        _store = store;
    }

    [HttpGet]
    public IActionResult GetStats()
    {
        var stats = _store.GetStats();
        var matches = _store.GetAllMatches();

        return Ok(new
        {
            success = true,
            data = new
            {
                totalMatches = stats.TotalMatches,
                totalMessages = stats.TotalMessages,
                completedMatches = stats.CompletedMatches,
                inProgressMatches = stats.InProgressMatches,
                scheduledMatches = stats.TotalMatches - stats.CompletedMatches - stats.InProgressMatches,
                stages = new
                {
                    group = matches.Count(m => m.Stage == Models.TournamentStage.Group),
                    roundOf16 = matches.Count(m => m.Stage == Models.TournamentStage.RoundOf16),
                    quarterFinal = matches.Count(m => m.Stage == Models.TournamentStage.QuarterFinal),
                    semiFinal = matches.Count(m => m.Stage == Models.TournamentStage.SemiFinal),
                    thirdPlace = matches.Count(m => m.Stage == Models.TournamentStage.ThirdPlace),
                    final = matches.Count(m => m.Stage == Models.TournamentStage.Final)
                }
            }
        });
    }
}
