using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TransferRepository : ITransferRepository
{
    private readonly FantasyPremierLeagueDbContext _context;

    public TransferRepository(FantasyPremierLeagueDbContext context)
    {
        _context = context;
    }

    public async Task<Transfer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Transfers.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Transfer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Transfers.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transfer>> GetByPlayerIdAsync(Guid playerId, CancellationToken cancellationToken = default)
    {
        return await _context.Transfers
            .Where(t => t.PlayerId == playerId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Transfer transfer, CancellationToken cancellationToken = default)
    {
        await _context.Transfers.AddAsync(transfer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
