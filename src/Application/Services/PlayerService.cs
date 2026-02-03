using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;

    public PlayerService(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<IEnumerable<PlayerDto>> GetAllPlayersAsync(CancellationToken cancellationToken = default)
    {
        var players = await _playerRepository.GetAllAsync(cancellationToken);
        return players.Select(MapToDto);
    }

    public async Task<PlayerDto?> GetPlayerByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var player = await _playerRepository.GetByIdAsync(id, cancellationToken);
        return player == null ? null : MapToDto(player);
    }

    public async Task<IEnumerable<PlayerDto>> GetPlayersByPositionAsync(string position, CancellationToken cancellationToken = default)
    {
        var players = await _playerRepository.GetByPositionAsync(position, cancellationToken);
        return players.Select(MapToDto);
    }

    public async Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto, CancellationToken cancellationToken = default)
    {
        var player = new Player(dto.Name, dto.Position, dto.Club, dto.Price);
        await _playerRepository.AddAsync(player, cancellationToken);
        return MapToDto(player);
    }

    public async Task<PlayerDto?> UpdatePlayerStatsAsync(Guid id, UpdatePlayerStatsDto dto, CancellationToken cancellationToken = default)
    {
        var player = await _playerRepository.GetByIdAsync(id, cancellationToken);
        if (player == null) return null;

        player.UpdateStats(dto.GoalsScored, dto.Assists, dto.CleanSheets);
        await _playerRepository.UpdateAsync(player, cancellationToken);
        return MapToDto(player);
    }

    public async Task<bool> DeletePlayerAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var player = await _playerRepository.GetByIdAsync(id, cancellationToken);
        if (player == null) return false;

        await _playerRepository.DeleteAsync(id, cancellationToken);
        return true;
    }

    private static PlayerDto MapToDto(Player player) => new(
        player.Id,
        player.Name,
        player.Position,
        player.Club,
        player.Price,
        player.Points,
        player.GoalsScored,
        player.Assists,
        player.CleanSheets
    );
}
