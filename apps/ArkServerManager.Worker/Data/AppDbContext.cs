using ArkServerManager.Worker.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArkServerManager.Worker.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ServerEntity> Servers => Set<ServerEntity>();
    public DbSet<JobEntity> Jobs => Set<JobEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServerEntity>(builder =>
        {
            builder.ToTable("servers");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Name).HasColumnName("name").IsRequired();
            builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at").IsRequired();
            builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at").IsRequired();
            builder.Property(x => x.State).HasColumnName("state").HasConversion<string>().IsRequired();
            builder.Property(x => x.InstallRoot).HasColumnName("install_root").IsRequired();
            builder.Property(x => x.GamePort).HasColumnName("game_port").IsRequired();
            builder.Property(x => x.QueryPort).HasColumnName("query_port").IsRequired();
            builder.Property(x => x.RconPort).HasColumnName("rcon_port").IsRequired();
            builder.Property(x => x.RconPassword).HasColumnName("rcon_password").IsRequired();
            builder.Property(x => x.MapName).HasColumnName("map_name").IsRequired();
            builder.Property(x => x.SessionName).HasColumnName("session_name").IsRequired();
            builder.Property(x => x.Pid).HasColumnName("pid");
            builder.Property(x => x.LastJobId).HasColumnName("last_job_id");
        });

        modelBuilder.Entity<JobEntity>(builder =>
        {
            builder.ToTable("jobs");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.ServerId).HasColumnName("server_id").IsRequired();
            builder.Property(x => x.Type).HasColumnName("type").HasConversion<string>().IsRequired();
            builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().IsRequired();
            builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at").IsRequired();
            builder.Property(x => x.StartedAtUtc).HasColumnName("started_at");
            builder.Property(x => x.FinishedAtUtc).HasColumnName("finished_at");
            builder.Property(x => x.ExitCode).HasColumnName("exit_code");
            builder.Property(x => x.LogBlob).HasColumnName("log_blob");

            builder.HasOne(x => x.Server)
                .WithMany(x => x.Jobs)
                .HasForeignKey(x => x.ServerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ServerId).HasDatabaseName("ix_jobs_server_id");
        });
    }
}
