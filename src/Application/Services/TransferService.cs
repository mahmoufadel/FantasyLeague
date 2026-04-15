using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class TransferService : ITransferService
{
    private readonly ITransferRepository _transferRepository;

    public TransferService(ITransferRepository transferRepository)
    {
        _transferRepository = transferRepository;
    }

    public async Task<IEnumerable<TransferDto>> GetAllTransfersAsync(CancellationToken cancellationToken = default)
    {
        var transfers = await _transferRepository.GetAllAsync(cancellationToken);
        return transfers.Select(MapToDto);
    }

    public async Task<TransferDto?> GetTransferByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var transfer = await _transferRepository.GetByIdAsync(id, cancellationToken);
        return transfer is null ? null : MapToDto(transfer);
    }

    public async Task<IEnumerable<TransferDto>> GetTransfersByPlayerAsync(Guid playerId, CancellationToken cancellationToken = default)
    {
        var transfers = await _transferRepository.GetByPlayerIdAsync(playerId, cancellationToken);
        return transfers.Select(MapToDto);
    }

    public async Task<TransferDto> CreateTransferAsync(CreateTransferDto dto, CancellationToken cancellationToken = default)
    {
        var transfer = new Transfer(dto.PlayerId, dto.FromTeamId, dto.ToTeamId, dto.TransferDate, dto.Fee);
        await _transferRepository.AddAsync(transfer, cancellationToken);
        return MapToDto(transfer);
    }

    private static TransferDto MapToDto(Transfer transfer) => new(
        transfer.Id,
        transfer.PlayerId,
        transfer.FromTeamId,
        transfer.ToTeamId,
        transfer.TransferDate,
        transfer.Fee
    );
}
