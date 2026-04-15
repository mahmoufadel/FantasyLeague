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
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IGameWeekService
{
    Task<IEnumerable<GameWeekDto>> GetAllGameWeeksAsync(CancellationToken cancellationToken = default);
    Task<GameWeekDto?> GetGameWeekByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GameWeekDto?> GetActiveGameWeekAsync(CancellationToken cancellationToken = default);
    Task<GameWeekDto> CreateGameWeekAsync(CreateGameWeekDto dto, CancellationToken cancellationToken = default);
    Task<GameWeekDto?> ActivateGameWeekAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GameWeekDto?> CompleteGameWeekAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeleteGameWeekAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IMatchResultRepository
{
    Task<MatchResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MatchResult>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(MatchResult matchResult, CancellationToken cancellationToken = default);
}

public interface IMatchResultService
{
    Task<MatchResultDto> CreateMatchResultAsync(CreateMatchResultDto dto, CancellationToken cancellationToken = default);
    Task<MatchResultDto?> GetMatchResultByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MatchResultDto>> GetAllMatchResultsAsync(CancellationToken cancellationToken = default);
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

public interface ITransferRepository
{
    Task<Transfer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transfer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Transfer>> GetByPlayerIdAsync(Guid playerId, CancellationToken cancellationToken = default);
    Task AddAsync(Transfer transfer, CancellationToken cancellationToken = default);
}

public interface ITransferService
{
    Task<IEnumerable<TransferDto>> GetAllTransfersAsync(CancellationToken cancellationToken = default);
    Task<TransferDto?> GetTransferByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TransferDto>> GetTransfersByPlayerAsync(Guid playerId, CancellationToken cancellationToken = default);
    Task<TransferDto> CreateTransferAsync(CreateTransferDto dto, CancellationToken cancellationToken = default);
}

