using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    string Name,

    [Required(ErrorMessage = "Position is required")]
    [RegularExpression("^(Goalkeeper|Defender|Midfielder|Forward)$",
        ErrorMessage = "Position must be one of: Goalkeeper, Defender, Midfielder, Forward")]
    string Position,

    [Required(ErrorMessage = "Club is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Club must be between 1 and 100 characters")]
    string Club,

    [Range(0.1, 999.9, ErrorMessage = "Price must be between 0.1 and 999.9")]
    decimal Price
);

public record UpdatePlayerStatsDto(
    [Range(0, int.MaxValue, ErrorMessage = "Goals scored cannot be negative")]
    int GoalsScored,

    [Range(0, int.MaxValue, ErrorMessage = "Assists cannot be negative")]
    int Assists,

    [Range(0, int.MaxValue, ErrorMessage = "Clean sheets cannot be negative")]
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
    [Required(ErrorMessage = "Team name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Team name must be between 1 and 100 characters")]
    string Name,

    [Required(ErrorMessage = "Manager name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Manager name must be between 1 and 100 characters")]
    string ManagerName
);

public record AddPlayerToTeamDto(
    [Required(ErrorMessage = "TeamId is required")]
    Guid TeamId,

    [Required(ErrorMessage = "PlayerId is required")]
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
    [Range(1, 38, ErrorMessage = "Week number must be between 1 and 38")]
    int WeekNumber,

    [Required(ErrorMessage = "Start date is required")]
    DateTime StartDate,

    [Required(ErrorMessage = "End date is required")]
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
    [Required(ErrorMessage = "HomeTeamId is required")]
    Guid HomeTeamId,

    [Required(ErrorMessage = "AwayTeamId is required")]
    Guid AwayTeamId,

    [Range(0, 99, ErrorMessage = "Home score must be between 0 and 99")]
    int HomeScore,

    [Range(0, 99, ErrorMessage = "Away score must be between 0 and 99")]
    int AwayScore,

    DateTime? MatchDate
);

public record TransferDto(
    Guid Id,
    Guid PlayerId,
    Guid FromTeamId,
    Guid ToTeamId,
    DateTime TransferDate,
    decimal Fee
);

public record CreateTransferDto(
    [Required(ErrorMessage = "PlayerId is required")]
    Guid PlayerId,

    [Required(ErrorMessage = "FromTeamId is required")]
    Guid FromTeamId,

    [Required(ErrorMessage = "ToTeamId is required")]
    Guid ToTeamId,

    [Required(ErrorMessage = "TransferDate is required")]
    DateTime TransferDate,

    [Range(0, double.MaxValue, ErrorMessage = "Fee cannot be negative")]
    decimal Fee
);

