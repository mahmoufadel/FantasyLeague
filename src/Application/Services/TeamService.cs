using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;
    private readonly IPlayerRepository _playerRepository;

    public TeamService(ITeamRepository teamRepository, IPlayerRepository playerRepository)
    {
        _teamRepository = teamRepository;
        _playerRepository = playerRepository;
    }

    public async Task<IEnumerable<TeamDto>> GetAllTeamsAsync(CancellationToken cancellationToken = default)
    {
        var teams = await _teamRepository.GetAllAsync(cancellationToken);
        var teamDtos = new List<TeamDto>();

        foreach (var team in teams)
        {
            teamDtos.Add(await MapToDto(team, cancellationToken));
        }

       
        return teamDtos;
    }

    public async Task<TeamDto?> GetTeamByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var team = await _teamRepository.GetByIdAsync(id, cancellationToken);
        return team == null ? null : await MapToDto(team, cancellationToken);
    }

    public async Task<TeamDto> CreateTeamAsync(CreateTeamDto dto, CancellationToken cancellationToken = default)
    {
        var team = new Team(dto.Name, dto.ManagerName);
        await _teamRepository.AddAsync(team, cancellationToken);
        return await MapToDto(team, cancellationToken);
    }

    public async Task<TeamDto?> AddPlayerToTeamAsync(AddPlayerToTeamDto dto, CancellationToken cancellationToken = default)
    {
        var team = await _teamRepository.GetByIdAsync(dto.TeamId, cancellationToken);
        if (team == null) return null;

        var player = await _playerRepository.GetByIdAsync(dto.PlayerId, cancellationToken);
        if (player == null) throw new InvalidOperationException("Player not found");

        team.AddPlayer(player);
        await _teamRepository.UpdateAsync(team, cancellationToken);
        return await MapToDto(team, cancellationToken);
    }

    public async Task<TeamDto?> RemovePlayerFromTeamAsync(Guid teamId, Guid playerId, CancellationToken cancellationToken = default)
    {
        var team = await _teamRepository.GetByIdAsync(teamId, cancellationToken);
        if (team == null) return null;

        var player = await _playerRepository.GetByIdAsync(playerId, cancellationToken);
        if (player == null) throw new InvalidOperationException("Player not found");

        team.RemovePlayer(playerId, player.Price);
        await _teamRepository.UpdateAsync(team, cancellationToken);
        return await MapToDto(team, cancellationToken);
    }

    public async Task<bool> DeleteTeamAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var team = await _teamRepository.GetByIdAsync(id, cancellationToken);
        if (team == null) return false;

        await _teamRepository.DeleteAsync(id, cancellationToken);
        return true;
    }

    private async Task<TeamDto> MapToDto(Team team, CancellationToken cancellationToken)
    {
        var players = new List<PlayerDto>();
        
        foreach (var teamPlayer in team.Players)
        {
            var player = await _playerRepository.GetByIdAsync(teamPlayer.PlayerId, cancellationToken);
            if (player != null)
            {
                players.Add(new PlayerDto(
                    player.Id,
                    player.Name,
                    player.Position,
                    player.Club,
                    player.Price,
                    player.Points,
                    player.GoalsScored,
                    player.Assists,
                    player.CleanSheets
                ));
            }
        }

        return new TeamDto(
            team.Id,
            team.Name,
            team.ManagerName,
            team.Budget,
            team.TotalPoints,
            players
        );
    }
}
