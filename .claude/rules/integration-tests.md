---
description: Integration test conventions using WebApplicationFactory and Testcontainers PostgreSQL
globs: ["tests/IntegrationTests/**/*.cs"]
alwaysApply: false
---

# Integration Test Rules

Integration tests exercise the full HTTP stack against a real PostgreSQL database spun up by Testcontainers.

## Naming Convention

Same as unit tests — no exceptions:
```
{MethodName}_When{Condition}_Should{ExpectedResult}
```

## Test Class Structure

```csharp
[Collection("PlayersApi")]          // shares one container across all tests in the collection
public class PlayersControllerTests : IClassFixture<PlayersApiFactory>, IAsyncLifetime
{
    private readonly PlayersApiFactory _factory;
    private readonly HttpClient _client;

    public PlayersControllerTests(PlayersApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;   // nothing needed before each test

    public async Task DisposeAsync()                        // wipe the table AFTER each test
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FantasyPremierLeagueDbContext>();
        db.Players.RemoveRange(db.Players);
        await db.SaveChangesAsync();
    }
}
```

## Seeding Data

Seed via `DbContext` directly — never through HTTP. Use a private helper:

```csharp
private async Task<PlayerDto> SeedPlayerAsync(
    string name = "Mohamed Salah",
    string position = "Forward",
    string club = "Liverpool",
    decimal price = 13.0m)
{
    using var scope = _factory.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<FantasyPremierLeagueDbContext>();
    var player = new Player(name, position, club, price);
    db.Players.Add(player);
    await db.SaveChangesAsync();
    return new PlayerDto(player.Id, player.Name, player.Position, player.Club,
        player.Price, player.Points, player.GoalsScored, player.Assists, player.CleanSheets);
}
```

## Writing Tests

- Use `[Fact]` — **not** `[Theory, AutoMockData]`
- Assert **both** the HTTP status code and the response body
- Use `response.Content.ReadFromJsonAsync<T>()` to deserialize the body
- Test unhappy paths (not found, bad request) as thoroughly as happy paths

```csharp
[Fact]
public async Task GetById_WhenPlayerExists_ShouldReturnOkWithPlayer()
{
    var seeded = await SeedPlayerAsync();

    var response = await _client.GetAsync($"/api/players/{seeded.Id}");

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var player = await response.Content.ReadFromJsonAsync<PlayerDto>();
    player.Should().NotBeNull();
    player!.Id.Should().Be(seeded.Id);
    player.Name.Should().Be(seeded.Name);
}
```

## `PlayersApiFactory` (`Fixtures/PlayersApiFactory.cs`)

- Inherits `WebApplicationFactory<Program>` and implements `IAsyncLifetime`
- Starts a `postgres:16-alpine` Testcontainers container in `InitializeAsync`
- Overrides the in-memory DbContext registration with `UseNpgsql(connectionString)` in `ConfigureWebHost`
- Runs `db.Database.EnsureCreated()` once after container start to apply the schema
- Shared across all tests via `[Collection]` to avoid per-test container startup cost

## Namespace

`namespace IntegrationTests;`
