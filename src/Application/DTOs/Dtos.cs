namespace Application.DTOs;

public record PlayerDto(
    Guid Id,
    string Name,
    string Position,
    string Club,
    decimal Price,
    int Points,
    int GoalsScored,
    int Assists,
    int CleanSheets
);

public record CreatePlayerDto(
    string Name,
    string Position,
    string Club,
    decimal Price
);

public record UpdatePlayerStatsDto(
    int GoalsScored,
    int Assists,
    int CleanSheets
);

public record TeamDto(
    Guid Id,
    string Name,
    string ManagerName,
    decimal Budget,
    int TotalPoints,
    List<PlayerDto> Players
);

public record CreateTeamDto(
    string Name,
    string ManagerName
);

public record AddPlayerToTeamDto(
    Guid TeamId,
    Guid PlayerId
);

public record GameWeekDto(
    Guid Id,
    int WeekNumber,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    bool IsCompleted
);

public record CreateGameWeekDto(
    int WeekNumber,
    DateTime StartDate,
    DateTime EndDate
);

public record MatchResultDto(
    Guid MatchId,
    DateTime MatchDate,
    Guid HomeTeamId,
    Guid AwayTeamId,
    int HomeScore,
    int AwayScore
);

public record CreateMatchResultDto(
    Guid HomeTeamId,
    Guid AwayTeamId,
    int HomeScore,
    int AwayScore,
    DateTime? MatchDate
);
