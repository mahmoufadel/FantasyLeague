using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class FantasyPremierLeagueDbContext : DbContext
{
    public DbSet<Player> Players { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<GameWeek> GameWeeks { get; set; }
    public DbSet<MatchResult> MatchResults { get; set; }
    public DbSet<Transfer> Transfers { get; set; }

    public FantasyPremierLeagueDbContext(DbContextOptions<FantasyPremierLeagueDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Player configuration
        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Position).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Club).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(10, 2);
        });

        // Team configuration
        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ManagerName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Budget).HasPrecision(10, 2);

            // Configure owned collection for TeamPlayers
            entity.OwnsMany(t => t.Players, tp =>
            {
                tp.WithOwner().HasForeignKey(nameof(TeamPlayer.TeamId));
                tp.Property<Guid>("Id");
                tp.HasKey("Id");
                tp.Property(x => x.PlayerId).IsRequired();
                tp.Property(x => x.AddedOn).IsRequired();
            });
        });

        // GameWeek configuration
        modelBuilder.Entity<GameWeek>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.WeekNumber).IsRequired();
            entity.Property(e => e.StartDate).IsRequired();
            entity.Property(e => e.EndDate).IsRequired();
        });

        // MatchResult configuration
        modelBuilder.Entity<MatchResult>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MatchDate).IsRequired();
            entity.Property(e => e.HomeTeamId).IsRequired();
            entity.Property(e => e.AwayTeamId).IsRequired();
            entity.Property(e => e.HomeScore).IsRequired();
            entity.Property(e => e.AwayScore).IsRequired();
        });

        // Transfer configuration
        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PlayerId).IsRequired();
            entity.Property(e => e.FromTeamId).IsRequired();
            entity.Property(e => e.ToTeamId).IsRequired();
            entity.Property(e => e.TransferDate).IsRequired();
            entity.Property(e => e.Fee).HasPrecision(10, 2);
        });
    }
}
