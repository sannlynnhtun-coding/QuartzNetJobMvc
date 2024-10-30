namespace QuartzNetJobMvc.Extensions.Quartz;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ServiceSetting> ServiceSettings { get; set; }
    public DbSet<Announcement> Announcements { get; set; }

    public override int SaveChanges()
    {
        try
        {
            var result = base.SaveChanges();
            Log.Information("Changes saved to the database.");
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving changes to the database.");
            throw; // Re-throw the exception after logging
        }
    }
}