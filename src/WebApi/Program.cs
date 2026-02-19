using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Fantasy Premier League API",
        Version = "v1",
        Description = "API for managing Fantasy Premier League teams and players"
    });
});

// Configure EF Core with In-Memory Database
builder.Services.AddDbContext<FantasyPremierLeagueDbContext>(options =>
    options.UseInMemoryDatabase("FantasyPremierLeagueDb"));

// Register repositories
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IGameWeekRepository, GameWeekRepository>();

// Register application services
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddSingleton<Dapr.Client.DaprClient>(sp => new Dapr.Client.DaprClientBuilder().Build());
builder.Services.AddScoped<Application.Interfaces.IMatchResultService, Application.Services.MatchResultService>();
builder.Services.AddScoped<WebApi.Services.DapprClient>();
builder.Services.AddScoped<Dapr.Client.DaprClient>(sp => sp.GetRequiredService<Dapr.Client.DaprClient>());

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    options.AddPolicy("AllowNgApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FantasyPremierLeagueDbContext>();
    SeedData(context);
}

// Enable Swagger in all environments (not just Development)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fantasy Premier League API v1");
    c.RoutePrefix = "swagger"; // Access at /swagger
});

app.UseCors("AllowReactApp");
app.UseCors("AllowNgApp");

app.UseAuthorization();
app.MapControllers();

// Log the URLs
var urls = app.Urls;
app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("==============================================");
    Console.WriteLine("Fantasy Premier League API is running!");
    Console.WriteLine("==============================================");
    Console.WriteLine($"Swagger UI: http://localhost:5000/swagger");
    Console.WriteLine($"API Base: http://localhost:5000/api");
    Console.WriteLine("==============================================");
});

app.Run();

static void SeedData(FantasyPremierLeagueDbContext context)
{
    if (context.Players.Any()) return;

    var players = new[]
    {
        new Domain.Entities.Player("Erling Haaland", "Forward", "Manchester City", 11.5m),
        new Domain.Entities.Player("Mohamed Salah", "Forward", "Liverpool", 13.0m),
        new Domain.Entities.Player("Kevin De Bruyne", "Midfielder", "Manchester City", 12.0m),
        new Domain.Entities.Player("Bruno Fernandes", "Midfielder", "Manchester United", 11.0m),
        new Domain.Entities.Player("Bukayo Saka", "Midfielder", "Arsenal", 9.5m),
        new Domain.Entities.Player("Virgil van Dijk", "Defender", "Liverpool", 6.5m),
        new Domain.Entities.Player("William Saliba", "Defender", "Arsenal", 6.0m),
        new Domain.Entities.Player("Trent Alexander-Arnold", "Defender", "Liverpool", 7.5m),
        new Domain.Entities.Player("Alisson Becker", "Goalkeeper", "Liverpool", 5.5m),
        new Domain.Entities.Player("Ederson", "Goalkeeper", "Manchester City", 5.0m),
        new Domain.Entities.Player("Son Heung-min", "Forward", "Tottenham", 10.0m),
        new Domain.Entities.Player("Phil Foden", "Midfielder", "Manchester City", 9.0m),
        new Domain.Entities.Player("Gabriel Jesus", "Forward", "Arsenal", 8.5m),
        new Domain.Entities.Player("Reece James", "Defender", "Chelsea", 6.0m),
        new Domain.Entities.Player("Aaron Ramsdale", "Goalkeeper", "Arsenal", 5.0m)
    };

   
    var teams = new[]
    { new Team("Barcelona", "Xavi") { }, // You must provide Id if it's required
            new Team("Real Madrid", "Carlo Ancelotti") { },
            new Team("Manchester United", "Erik Ten Hag") { } };
   
    context.Teams.AddRange(teams);
    context.Players.AddRange(players);
    context.SaveChanges();
    Console.WriteLine($"Seeded {players.Length} players into the database");
}