using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GameWeekRepository : IGameWeekRepository
{
    private readonly FantasyPremierLeagueDbContext _context;

    public GameWeekRepository(FantasyPremierLeagueDbContext context)
    {
        _context = context;
    }

    public async Task<GameWeek?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.GameWeeks.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<GameWeek>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.GameWeeks
            .OrderBy(gw => gw.WeekNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<GameWeek?> GetActiveGameWeekAsync(CancellationToken cancellationToken = default)
    {
        return await _context.GameWeeks
            .FirstOrDefaultAsync(gw => gw.IsActive, cancellationToken);
    }

    public async Task AddAsync(GameWeek gameWeek, CancellationToken cancellationToken = default)
    {
        await _context.GameWeeks.AddAsync(gameWeek, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(GameWeek gameWeek, CancellationToken cancellationToken = default)
    {
        _context.GameWeeks.Update(gameWeek);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
