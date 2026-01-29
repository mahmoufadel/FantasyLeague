using Application.DTOs;
using Application.Interfaces;

namespace Application.Services;

public class MatchResultService : IMatchResultService
{
    // The application service doesn't perform persistence here, it will delegate to infrastructure or publish events.
    public Task<MatchResultDto> CreateMatchResultAsync(CreateMatchResultDto dto, CancellationToken cancellationToken = default)
    {
        var matchDate = dto.MatchDate ?? DateTime.UtcNow;
        var result = new MatchResultDto(Guid.NewGuid(), matchDate, dto.HomeTeamId, dto.AwayTeamId, dto.HomeScore, dto.AwayScore);
        return Task.FromResult(result);
    }
}
