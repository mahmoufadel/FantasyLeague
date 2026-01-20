using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly FantasyPremierLeagueDbContext _context;

    public PlayerRepository(FantasyPremierLeagueDbContext context)
    {
        _context = context;
    }

    public async Task<Player?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Players.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Player>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Players.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Player>> GetByPositionAsync(string position, CancellationToken cancellationToken = default)
    {
        return await _context.Players
            .Where(p => p.Position == position)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Player player, CancellationToken cancellationToken = default)
    {
        await _context.Players.AddAsync(player, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Player player, CancellationToken cancellationToken = default)
    {
        _context.Players.Update(player);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var player = await GetByIdAsync(id, cancellationToken);
        if (player != null)
        {
            _context.Players.Remove(player);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
