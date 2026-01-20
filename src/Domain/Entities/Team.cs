using Domain.Common;

namespace Domain.Entities;

public class Team : Entity
{
    public string Name { get; private set; }
    public string ManagerName { get; private set; }
    public decimal Budget { get; private set; }
    public int TotalPoints { get; private set; }
    
    private readonly List<TeamPlayer> _players = new();
    public IReadOnlyCollection<TeamPlayer> Players => _players.AsReadOnly();

    private const decimal InitialBudget = 100m;
    private const int MaxPlayers = 15;

    private Team() { } // For EF Core

    public Team(string name, string managerName)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ManagerName = managerName ?? throw new ArgumentNullException(nameof(managerName));
        Budget = InitialBudget;
        TotalPoints = 0;
    }

    public void AddPlayer(Player player)
    {
        if (player == null)
            throw new ArgumentNullException(nameof(player));

        if (_players.Count >= MaxPlayers)
            throw new InvalidOperationException($"Cannot have more than {MaxPlayers} players");

        if (Budget < player.Price)
            throw new InvalidOperationException("Insufficient budget to add this player");

        if (_players.Any(tp => tp.PlayerId == player.Id))
            throw new InvalidOperationException("Player already in team");

        _players.Add(new TeamPlayer(Id, player.Id));
        Budget -= player.Price;
    }

    public void RemovePlayer(Guid playerId, decimal playerPrice)
    {
        var teamPlayer = _players.FirstOrDefault(tp => tp.PlayerId == playerId);
        if (teamPlayer == null)
            throw new InvalidOperationException("Player not in team");

        _players.Remove(teamPlayer);
        Budget += playerPrice;
    }

    public void UpdatePoints(int points)
    {
        TotalPoints += points;
    }
}

public class TeamPlayer
{
    public Guid TeamId { get; private set; }
    public Guid PlayerId { get; private set; }
    public DateTime AddedOn { get; private set; }

    private TeamPlayer() { } // For EF Core

    public TeamPlayer(Guid teamId, Guid playerId)
    {
        TeamId = teamId;
        PlayerId = playerId;
        AddedOn = DateTime.UtcNow;
    }
}
