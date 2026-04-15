using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class MatchResultService : IMatchResultService
{
    private readonly IMatchResultRepository _matchResultRepository;

    public MatchResultService(IMatchResultRepository matchResultRepository)
    {
        _matchResultRepository = matchResultRepository;
    }

    public async Task<MatchResultDto> CreateMatchResultAsync(CreateMatchResultDto dto, CancellationToken cancellationToken = default)
    {
        var matchDate = dto.MatchDate ?? DateTime.UtcNow;
        var matchResult = new MatchResult(matchDate, dto.HomeTeamId, dto.AwayTeamId, dto.HomeScore, dto.AwayScore);

        await _matchResultRepository.AddAsync(matchResult, cancellationToken);

        return MapToDto(matchResult);
    }

    public async Task<MatchResultDto?> GetMatchResultByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var matchResult = await _matchResultRepository.GetByIdAsync(id, cancellationToken);
        return matchResult is null ? null : MapToDto(matchResult);
    }

    public async Task<IEnumerable<MatchResultDto>> GetAllMatchResultsAsync(CancellationToken cancellationToken = default)
    {
        var results = await _matchResultRepository.GetAllAsync(cancellationToken);
        return results.Select(MapToDto);
    }

    private static MatchResultDto MapToDto(MatchResult matchResult) =>
        new(matchResult.Id, matchResult.MatchDate, matchResult.HomeTeamId,
            matchResult.AwayTeamId, matchResult.HomeScore, matchResult.AwayScore);
}

