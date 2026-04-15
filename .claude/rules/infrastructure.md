---
description: EF Core DbContext and repository implementation conventions for the Infrastructure layer
globs: ["src/Infrastructure/**/*.cs"]
alwaysApply: false
---

# Infrastructure Layer Rules

The Infrastructure layer implements the interfaces defined in Application. It owns all EF Core and persistence details.

## Repository Pattern

- Implement `IXxxRepository` from `Application/Interfaces/`
- Constructor-inject `FantasyPremierLeagueDbContext`
- All methods are `async` and forward `CancellationToken` to EF Core calls

```csharp
public class PlayerRepository : IPlayerRepository
{
    private readonly FantasyPremierLeagueDbContext _context;

    public PlayerRepository(FantasyPremierLeagueDbContext context)
    {
        _context = context;
    }

    public async Task<Player?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Players.FindAsync([id], ct);

    public async Task AddAsync(Player player, CancellationToken ct = default)
    {
        await _context.Players.AddAsync(player, ct);
        await _context.SaveChangesAsync(ct);
    }
}
```

## DbContext (`FantasyPremierLeagueDbContext`)

- Defined in `Infrastructure/Persistence/`
- Each aggregate root gets its own `DbSet<T>`
- Use Fluent API in `OnModelCreating` for configuration — no data annotations on domain entities
- Seed data goes in `OnModelCreating` via `modelBuilder.Entity<T>().HasData(...)`

## Database Registration

The app currently uses **EF Core In-Memory** for development:
```csharp
services.AddDbContext<FantasyPremierLeagueDbContext>(opt =>
    opt.UseInMemoryDatabase("FplDb"));
```

Integration tests replace this with a real **PostgreSQL** container via Testcontainers — do not hardcode the connection string here.

To switch to SQL Server for production: replace `UseInMemoryDatabase` with `UseSqlServer(connectionString)` in `Program.cs`.

## Forbidden in Infrastructure

- No business logic — infrastructure only persists and retrieves
- No direct references to `WebApi` or `Application.Services`
- No DTOs — work exclusively with domain entities
