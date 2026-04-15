using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using FluentAssertions;
using IntegrationTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence;
using Domain.Entities;

namespace IntegrationTests;

/// <summary>
/// Integration tests for PlayersController backed by a real PostgreSQL instance
/// running inside a Testcontainers Docker container.
///
/// Each test class shares one factory (and therefore one container) via
/// IClassFixture to keep total test runtime short.
/// The database is reset between tests so each test is fully isolated.
/// </summary>
[Collection("PlayersApi")]
public class PlayersControllerTests : IClassFixture<PlayersApiFactory>, IAsyncLifetime
{
    private readonly PlayersApiFactory _factory;
    private readonly HttpClient _client;

    public PlayersControllerTests(PlayersApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // ── lifecycle ──────────────────────────────────────────────────────────

    public Task InitializeAsync() => Task.CompletedTask;

    /// <summary>Wipe the Players table after every test so tests don't bleed into each other.</summary>
    public async Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FantasyPremierLeagueDbContext>();
        db.Players.RemoveRange(db.Players);
        await db.SaveChangesAsync();
    }

    // ── helpers ────────────────────────────────────────────────────────────

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

    // ── GET /api/players ───────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_WhenNoPlayers_ShouldReturnOkWithEmptyList()
    {
        var response = await _client.GetAsync("/api/players");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var players = await response.Content.ReadFromJsonAsync<List<PlayerDto>>();
        players.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task GetAll_WhenPlayersExist_ShouldReturnAllPlayers()
    {
        await SeedPlayerAsync("Erling Haaland", "Forward", "Manchester City", 11.5m);
        await SeedPlayerAsync("Kevin De Bruyne", "Midfielder", "Manchester City", 12.0m);

        var response = await _client.GetAsync("/api/players");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var players = await response.Content.ReadFromJsonAsync<List<PlayerDto>>();
        players.Should().HaveCount(2);
        players!.Select(p => p.Name).Should().Contain("Erling Haaland").And.Contain("Kevin De Bruyne");
    }

    // ── GET /api/players/{id} ──────────────────────────────────────────────

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
        player.Position.Should().Be(seeded.Position);
        player.Club.Should().Be(seeded.Club);
        player.Price.Should().Be(seeded.Price);
    }

    [Fact]
    public async Task GetById_WhenPlayerDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync($"/api/players/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── GET /api/players/position/{position} ──────────────────────────────

    [Fact]
    public async Task GetByPosition_WhenMatchingPlayersExist_ShouldReturnFilteredList()
    {
        await SeedPlayerAsync("Virgil van Dijk", "Defender", "Liverpool", 6.5m);
        await SeedPlayerAsync("William Saliba", "Defender", "Arsenal", 6.0m);
        await SeedPlayerAsync("Erling Haaland", "Forward", "Manchester City", 11.5m);

        var response = await _client.GetAsync("/api/players/position/Defender");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var players = await response.Content.ReadFromJsonAsync<List<PlayerDto>>();
        players.Should().HaveCount(2).And.OnlyContain(p => p.Position == "Defender");
    }

    [Fact]
    public async Task GetByPosition_WhenNoMatchingPlayers_ShouldReturnOkWithEmptyList()
    {
        await SeedPlayerAsync("Alisson Becker", "Goalkeeper", "Liverpool", 5.5m);

        var response = await _client.GetAsync("/api/players/position/Forward");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var players = await response.Content.ReadFromJsonAsync<List<PlayerDto>>();
        players.Should().NotBeNull().And.BeEmpty();
    }

    // ── POST /api/players ─────────────────────────────────────────────────

    [Fact]
    public async Task Create_WhenDtoIsValid_ShouldReturnCreatedWithPlayer()
    {
        var dto = new CreatePlayerDto("Bukayo Saka", "Midfielder", "Arsenal", 9.5m);

        var response = await _client.PostAsJsonAsync("/api/players", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var player = await response.Content.ReadFromJsonAsync<PlayerDto>();
        player.Should().NotBeNull();
        player!.Id.Should().NotBeEmpty();
        player.Name.Should().Be("Bukayo Saka");
        player.Position.Should().Be("Midfielder");
        player.Club.Should().Be("Arsenal");
        player.Price.Should().Be(9.5m);
        player.Points.Should().Be(0);

        // Location header should point to GET /api/players/{id}
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain(player.Id.ToString());
    }

    [Fact]
    public async Task Create_WhenNameIsEmpty_ShouldReturnBadRequest()
    {
        var dto = new CreatePlayerDto("", "Forward", "Arsenal", 9.5m);

        var response = await _client.PostAsJsonAsync("/api/players", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WhenPositionIsInvalid_ShouldReturnBadRequest()
    {
        var dto = new CreatePlayerDto("Test Player", "Striker", "Arsenal", 9.5m);

        var response = await _client.PostAsJsonAsync("/api/players", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WhenPriceIsZero_ShouldReturnBadRequest()
    {
        var dto = new CreatePlayerDto("Test Player", "Forward", "Arsenal", 0m);

        var response = await _client.PostAsJsonAsync("/api/players", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WhenPriceIsNegative_ShouldReturnBadRequest()
    {
        var dto = new CreatePlayerDto("Test Player", "Forward", "Arsenal", -5m);

        var response = await _client.PostAsJsonAsync("/api/players", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ── PUT /api/players/{id}/stats ────────────────────────────────────────

    [Fact]
    public async Task UpdateStats_WhenPlayerExistsAndStatsAreValid_ShouldReturnOkWithUpdatedPoints()
    {
        var seeded = await SeedPlayerAsync();

        var dto = new UpdatePlayerStatsDto(GoalsScored: 2, Assists: 1, CleanSheets: 0);
        var response = await _client.PutAsJsonAsync($"/api/players/{seeded.Id}/stats", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var player = await response.Content.ReadFromJsonAsync<PlayerDto>();
        player.Should().NotBeNull();
        player!.GoalsScored.Should().Be(2);
        player.Assists.Should().Be(1);
        player.CleanSheets.Should().Be(0);
        // 2 goals × 5 + 1 assist × 3 + 0 clean sheets × 4 = 13
        player.Points.Should().Be(13);
    }

    [Fact]
    public async Task UpdateStats_WhenPlayerDoesNotExist_ShouldReturnNotFound()
    {
        var dto = new UpdatePlayerStatsDto(GoalsScored: 1, Assists: 0, CleanSheets: 0);

        var response = await _client.PutAsJsonAsync($"/api/players/{Guid.NewGuid()}/stats", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateStats_WhenGoalsScoredIsNegative_ShouldReturnBadRequest()
    {
        var seeded = await SeedPlayerAsync();
        var dto = new UpdatePlayerStatsDto(GoalsScored: -1, Assists: 0, CleanSheets: 0);

        var response = await _client.PutAsJsonAsync($"/api/players/{seeded.Id}/stats", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateStats_CanBeCalledMultipleTimes_ShouldAccumulateStats()
    {
        var seeded = await SeedPlayerAsync();

        await _client.PutAsJsonAsync($"/api/players/{seeded.Id}/stats",
            new UpdatePlayerStatsDto(GoalsScored: 1, Assists: 0, CleanSheets: 0));

        var response = await _client.PutAsJsonAsync($"/api/players/{seeded.Id}/stats",
            new UpdatePlayerStatsDto(GoalsScored: 1, Assists: 1, CleanSheets: 0));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var player = await response.Content.ReadFromJsonAsync<PlayerDto>();
        player!.GoalsScored.Should().Be(2);
        player.Assists.Should().Be(1);
        // 2 goals × 5 + 1 assist × 3 = 13
        player.Points.Should().Be(13);
    }

    // ── DELETE /api/players/{id} ───────────────────────────────────────────

    [Fact]
    public async Task Delete_WhenPlayerExists_ShouldReturnNoContent()
    {
        var seeded = await SeedPlayerAsync();

        var response = await _client.DeleteAsync($"/api/players/{seeded.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Confirm the player is actually gone
        var getResponse = await _client.GetAsync($"/api/players/{seeded.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenPlayerDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.DeleteAsync($"/api/players/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
