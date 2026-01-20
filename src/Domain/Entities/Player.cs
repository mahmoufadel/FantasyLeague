using Domain.Common;

namespace Domain.Entities;

public class Player : Entity
{
    public string Name { get; private set; }
    public string Position { get; private set; }
    public string Club { get; private set; }
    public decimal Price { get; private set; }
    public int Points { get; private set; }
    public int GoalsScored { get; private set; }
    public int Assists { get; private set; }
    public int CleanSheets { get; private set; }

    private Player() { } // For EF Core

    public Player(string name, string position, string club, decimal price)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Position = position ?? throw new ArgumentNullException(nameof(position));
        Club = club ?? throw new ArgumentNullException(nameof(club));
        
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(price));
        
        Price = price;
        Points = 0;
        GoalsScored = 0;
        Assists = 0;
        CleanSheets = 0;
    }

    public void UpdateStats(int goalsScored, int assists, int cleanSheets)
    {
        GoalsScored += goalsScored;
        Assists += assists;
        CleanSheets += cleanSheets;
        
        // Simple points calculation
        Points += (goalsScored * 5) + (assists * 3) + (cleanSheets * 4);
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(newPrice));
        
        Price = newPrice;
    }
}
