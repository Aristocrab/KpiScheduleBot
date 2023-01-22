using Microsoft.EntityFrameworkCore;

namespace KpiSchedule.Database;

public sealed class KpiScheduleDbContext : DbContext
{
    public DbSet<ChatSettings> ChatsSettings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatSettings>(
            eb =>
            {
                eb.HasKey(x => x.ChatId);
            });
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { 
        optionsBuilder.UseSqlite("Data Source=kpischedule.db");
    }

    public KpiScheduleDbContext()
    {
        Database.EnsureCreated();
    }
}