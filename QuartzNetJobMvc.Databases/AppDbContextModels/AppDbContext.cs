using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuartzNetJobMvc.Databases.AppDbContextModels;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Announcement> Announcements { get; set; }

    public virtual DbSet<QuartzLog> QuartzLogs { get; set; }

    public virtual DbSet<ScheduleSetting> ScheduleSettings { get; set; }

    public virtual DbSet<ServiceSetting> ServiceSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Announce__3214EC07CEB068B9");

            entity.ToTable("Announcement");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<QuartzLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Logs__3214EC073FDE4AA6");

            entity.Property(e => e.Level).HasMaxLength(128);
        });

        modelBuilder.Entity<ScheduleSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC070F54CBF6");

            entity.Property(e => e.CronExpression).HasMaxLength(100);
            entity.Property(e => e.JobType).HasMaxLength(100);
            entity.Property(e => e.ServiceName).HasMaxLength(255);
        });

        modelBuilder.Entity<ServiceSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ServiceS__3214EC072E67DD8F");

            entity.ToTable("ServiceSetting");

            entity.Property(e => e.ServiceName).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
