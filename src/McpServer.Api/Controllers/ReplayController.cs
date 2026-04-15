using McpServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace McpServer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReplayController : ControllerBase
{
    private readonly WorldCupStore _store;

    public ReplayController(WorldCupStore store)
    {
        _store = store;
    }

    [HttpPost]
    public IActionResult CreateSession([FromBody] CreateReplayRequest request)
    {
        var match = _store.GetMatch(request.MatchId);
        if (match == null)
            return NotFound(new { success = false, error = "Match not found" });

        var session = _store.CreateReplaySession(request.MatchId);
        if (session == null)
            return BadRequest(new { success = false, error = "Match has no messages to replay" });

        return CreatedAtAction(nameof(GetStatus), new { sessionId = session.SessionId },
            new
            {
                success = true,
                message = "Replay session created",
                data = new
                {
                    sessionId = session.SessionId,
                    matchId = session.MatchId,
                    totalMessages = session.Messages.Count,
                    messages = session.Messages
                }
            });
    }

    [HttpGet("{sessionId}")]
    public IActionResult GetStatus(string sessionId)
    {
        var status = _store.GetReplayStatus(sessionId);
        if (status == null)
            return NotFound(new { success = false, error = "Session not found" });

        return Ok(new { success = true, data = status });
    }

    [HttpPost("{sessionId}/start")]
    public IActionResult Start(string sessionId, [FromBody] StartReplayRequest? request)
    {
        var session = _store.StartReplay(sessionId, request?.Speed ?? 1.0);
        if (session == null)
            return NotFound(new { success = false, error = "Session not found" });

        return Ok(new
        {
            success = true,
            message = "Replay started",
            data = new
            {
                sessionId = session.SessionId,
                isPlaying = session.IsPlaying,
                currentIndex = session.CurrentIndex,
                totalMessages = session.Messages.Count,
                speed = session.Speed
            }
        });
    }

    [HttpPost("{sessionId}/pause")]
    public IActionResult Pause(string sessionId)
    {
        var session = _store.PauseReplay(sessionId);
        if (session == null)
            return NotFound(new { success = false, error = "Session not found" });

        return Ok(new
        {
            success = true,
            message = "Replay paused",
            data = new { sessionId = session.SessionId, isPlaying = session.IsPlaying }
        });
    }

    [HttpPost("{sessionId}/next")]
    public IActionResult Next(string sessionId)
    {
        var result = _store.GetNextMessage(sessionId);
        if (result == null)
            return NotFound(new { success = false, error = "Session not found" });

        var (session, message) = result.Value;

        if (message == null)
        {
            return Ok(new
            {
                success = true,
                message = "Replay completed",
                data = new
                {
                    sessionId = session.SessionId,
                    currentIndex = session.CurrentIndex,
                    totalMessages = session.Messages.Count,
                    isPlaying = false,
                    hasMore = false
                }
            });
        }

        return Ok(new
        {
            success = true,
            message = "Next message retrieved",
            data = new
            {
                session = new
                {
                    sessionId = session.SessionId,
                    currentIndex = session.CurrentIndex,
                    totalMessages = session.Messages.Count,
                    isPlaying = session.IsPlaying,
                    progress = session.GetProgress()
                },
                message,
                hasMore = session.CurrentIndex < session.Messages.Count
            }
        });
    }

    [HttpPost("{sessionId}/stop")]
    public IActionResult Stop(string sessionId)
    {
        var session = _store.StopReplay(sessionId);
        if (session == null)
            return NotFound(new { success = false, error = "Session not found" });

        return Ok(new
        {
            success = true,
            message = "Replay stopped and reset",
            data = new
            {
                sessionId = session.SessionId,
                currentIndex = session.CurrentIndex,
                isPlaying = session.IsPlaying
            }
        });
    }

    [HttpPost("{sessionId}/reset")]
    public IActionResult Reset(string sessionId)
    {
        var session = _store.ResetReplay(sessionId);
        if (session == null)
            return NotFound(new { success = false, error = "Session not found" });

        return Ok(new
        {
            success = true,
            message = "Replay reset to beginning",
            data = new
            {
                sessionId = session.SessionId,
                currentIndex = session.CurrentIndex,
                isPlaying = session.IsPlaying
            }
        });
    }
}

public class CreateReplayRequest
{
    public string MatchId { get; set; } = string.Empty;
}

public class StartReplayRequest
{
    public double Speed { get; set; } = 1.0;
}
