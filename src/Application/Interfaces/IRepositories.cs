using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface IPlayerRepository
{
    Task<Player?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Player>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Player>> GetByPositionAsync(string position, CancellationToken cancellationToken = default);
    Task AddAsync(Player player, CancellationToken cancellationToken = default);
    Task UpdateAsync(Player player, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Team>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Team?> GetByManagerNameAsync(string managerName, CancellationToken cancellationToken = default);
    Task AddAsync(Team team, CancellationToken cancellationToken = default);
    Task UpdateAsync(Team team, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IGameWeekRepository
{
    Task<GameWeek?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<GameWeek>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GameWeek?> GetActiveGameWeekAsync(CancellationToken cancellationToken = default);
    Task AddAsync(GameWeek gameWeek, CancellationToken cancellationToken = default);
    Task UpdateAsync(GameWeek gameWeek, CancellationToken cancellationToken = default);
}

public interface IMatchResultService
{
    Task<MatchResultDto> CreateMatchResultAsync(CreateMatchResultDto dto, CancellationToken cancellationToken = default);
}
