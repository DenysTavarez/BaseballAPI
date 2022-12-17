using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace BaseballModel.Models;

public partial class BaseballStatsContext : IdentityDbContext<BaseballUser>
{
    public BaseballStatsContext()
    {
    }

    public BaseballStatsContext(DbContextOptions<BaseballStatsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
            if (!optionsBuilder.IsConfigured)
            
                {
                    IConfigurationBuilder builder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: false)
                        .AddJsonFile("appsettings.development.json", optional: true);
                    IConfigurationRoot configuration = builder.Build();
                    optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

                }
    }
protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasOne(d => d.Team)
                .WithMany(p => p.Players)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Player_Team");
        });
        modelBuilder.Entity<Team>(entity =>
        {
            entity.Property(e => e.TeamName).IsFixedLength();

            entity.Property(e => e.TeamRank).IsFixedLength();
        });

      
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
