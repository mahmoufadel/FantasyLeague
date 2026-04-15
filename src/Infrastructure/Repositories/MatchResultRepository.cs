using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MatchResultRepository : IMatchResultRepository
{
    private readonly FantasyPremierLeagueDbContext _context;

    public MatchResultRepository(FantasyPremierLeagueDbContext context)
    {
        _context = context;
    }

    public async Task<MatchResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MatchResults.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<MatchResult>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MatchResults.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MatchResult matchResult, CancellationToken cancellationToken = default)
    {
        await _context.MatchResults.AddAsync(matchResult, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
