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

public interface ITeamService
{
    Task<IEnumerable<TeamDto>> GetAllTeamsAsync(CancellationToken cancellationToken = default);
    Task<TeamDto?> GetTeamByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TeamDto> CreateTeamAsync(CreateTeamDto dto, CancellationToken cancellationToken = default);
    Task<TeamDto?> AddPlayerToTeamAsync(AddPlayerToTeamDto dto, CancellationToken cancellationToken = default);
    Task<TeamDto?> RemovePlayerFromTeamAsync(Guid teamId, Guid playerId, CancellationToken cancellationToken = default);
    Task<bool> DeleteTeamAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IPlayerService
{
    Task<IEnumerable<PlayerDto>> GetAllPlayersAsync(CancellationToken cancellationToken = default);
    Task<PlayerDto?> GetPlayerByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PlayerDto>> GetPlayersByPositionAsync(string position, CancellationToken cancellationToken = default);
    Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto, CancellationToken cancellationToken = default);
    Task<PlayerDto?> UpdatePlayerStatsAsync(Guid id, UpdatePlayerStatsDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeletePlayerAsync(Guid id, CancellationToken cancellationToken = default);
}
