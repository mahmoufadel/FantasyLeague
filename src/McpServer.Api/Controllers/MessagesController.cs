using McpServer.Models;
using McpServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace McpServer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly WorldCupStore _store;

    public MessagesController(WorldCupStore store)
    {
        _store = store;
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateMessageRequest request)
    {
        try
        {
            if (!Enum.TryParse<MessageType>(request.MessageType, true, out var type))
            {
                return BadRequest(new
                {
                    success = false,
                    error = $"Invalid message type. Valid values: {string.Join(", ", Enum.GetNames<MessageType>())}"
                });
            }

            var match = _store.GetMatch(request.MatchId);
            if (match == null)
                return NotFound(new { success = false, error = "Match not found" });

            var message = _store.AddMessage(
                request.UserId,
                request.Username,
                request.MatchId,
                request.Content,
                type
            );

            return CreatedAtAction(nameof(GetById), new { id = message.Id },
                new { success = true, message = "Message added successfully", data = message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var message = _store.GetMessage(id);
        if (message == null)
            return NotFound(new { success = false, error = "Message not found" });

        return Ok(new { success = true, data = message });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var deleted = _store.DeleteMessage(id);
        if (!deleted)
            return NotFound(new { success = false, error = "Message not found" });

        return Ok(new { success = true, message = "Message deleted successfully" });
    }

    [HttpGet("user/{userId}")]
    public IActionResult GetByUser(string userId)
    {
        var messages = _store.GetMessagesByUser(userId);
        return Ok(new { success = true, count = messages.Count, data = messages });
    }
}

public class CreateMessageRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string MatchId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string MessageType { get; set; } = string.Empty;
}
