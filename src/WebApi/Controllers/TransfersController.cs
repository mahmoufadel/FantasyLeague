using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransfersController : ControllerBase
{
    private readonly ITransferService _transferService;

    public TransfersController(ITransferService transferService)
    {
        _transferService = transferService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransferDto>>> GetAll(CancellationToken cancellationToken)
    {
        var transfers = await _transferService.GetAllTransfersAsync(cancellationToken);
        return Ok(transfers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TransferDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var transfer = await _transferService.GetTransferByIdAsync(id, cancellationToken);
        if (transfer is null)
            return NotFound();

        return Ok(transfer);
    }

    [HttpGet("player/{playerId}")]
    public async Task<ActionResult<IEnumerable<TransferDto>>> GetByPlayer(Guid playerId, CancellationToken cancellationToken)
    {
        var transfers = await _transferService.GetTransfersByPlayerAsync(playerId, cancellationToken);
        return Ok(transfers);
    }

    [HttpPost]
    public async Task<ActionResult<TransferDto>> Create(CreateTransferDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var transfer = await _transferService.CreateTransferAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = transfer.Id }, transfer);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
