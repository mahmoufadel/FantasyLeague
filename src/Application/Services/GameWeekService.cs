using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class GameWeekService : IGameWeekService
{
    private readonly IGameWeekRepository _gameWeekRepository;

    public GameWeekService(IGameWeekRepository gameWeekRepository)
    {
        _gameWeekRepository = gameWeekRepository;
    }

    public async Task<IEnumerable<GameWeekDto>> GetAllGameWeeksAsync(CancellationToken cancellationToken = default)
    {
        var gameWeeks = await _gameWeekRepository.GetAllAsync(cancellationToken);
        return gameWeeks.Select(MapToDto);
    }

    public async Task<GameWeekDto?> GetGameWeekByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var gameWeek = await _gameWeekRepository.GetByIdAsync(id, cancellationToken);
        return gameWeek is null ? null : MapToDto(gameWeek);
    }

    public async Task<GameWeekDto?> GetActiveGameWeekAsync(CancellationToken cancellationToken = default)
    {
        var gameWeek = await _gameWeekRepository.GetActiveGameWeekAsync(cancellationToken);
        return gameWeek is null ? null : MapToDto(gameWeek);
    }

    public async Task<GameWeekDto> CreateGameWeekAsync(CreateGameWeekDto dto, CancellationToken cancellationToken = default)
    {
        var gameWeek = new GameWeek(dto.WeekNumber, dto.StartDate, dto.EndDate);
        await _gameWeekRepository.AddAsync(gameWeek, cancellationToken);
        return MapToDto(gameWeek);
    }

    public async Task<GameWeekDto?> ActivateGameWeekAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var gameWeek = await _gameWeekRepository.GetByIdAsync(id, cancellationToken);
        if (gameWeek is null)
            return null;

        gameWeek.Activate();
        await _gameWeekRepository.UpdateAsync(gameWeek, cancellationToken);
        return MapToDto(gameWeek);
    }

    public async Task<GameWeekDto?> CompleteGameWeekAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var gameWeek = await _gameWeekRepository.GetByIdAsync(id, cancellationToken);
        if (gameWeek is null)
            return null;

        gameWeek.Complete();
        await _gameWeekRepository.UpdateAsync(gameWeek, cancellationToken);
        return MapToDto(gameWeek);
    }

    public async Task<bool> DeleteGameWeekAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var gameWeek = await _gameWeekRepository.GetByIdAsync(id, cancellationToken);
        if (gameWeek is null)
            return false;

        await _gameWeekRepository.DeleteAsync(id, cancellationToken);
        return true;
    }

    private static GameWeekDto MapToDto(GameWeek gameWeek) =>
        new(gameWeek.Id, gameWeek.WeekNumber, gameWeek.StartDate, gameWeek.EndDate, gameWeek.IsActive, gameWeek.IsCompleted);
}
