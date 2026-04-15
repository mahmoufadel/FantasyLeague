using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace IntegrationTests.Fixtures;

/// <summary>
/// Spins up a real PostgreSQL container via Testcontainers and hosts the WebApi
/// inside an in-process test server. Shared across all tests in the collection
/// to avoid paying container-startup cost per test.
/// </summary>
public class PlayersApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("fpl_test")
        .WithUsername("fpl_user")
        .WithPassword("fpl_pass")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgres.StopAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the in-memory DbContext registration from Program.cs
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<FantasyPremierLeagueDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            // Re-register with the real PostgreSQL container connection string
            services.AddDbContext<FantasyPremierLeagueDbContext>(options =>
                options.UseNpgsql(_postgres.GetConnectionString()));
        });

        builder.UseEnvironment("Testing");

        // Run migrations to create the schema before tests start
        builder.ConfigureServices(services =>
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<FantasyPremierLeagueDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
