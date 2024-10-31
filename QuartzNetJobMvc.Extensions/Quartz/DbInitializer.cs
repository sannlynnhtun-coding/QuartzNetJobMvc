namespace QuartzNetJobMvc.Extensions.Quartz;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        try
        {
            if (context.ServiceSettings.Any())
            {
                Log.Information("DB has already been seeded.");
                return; // DB has been seeded
            }

            // Seed initial email service setting
            context.ServiceSettings.Add(new ServiceSetting
            {
                ServiceName = "EmailService",
                IsEnabled = true
            });

            context.SaveChanges();
            Log.Information("Database initialized and email service settings seeded.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error initializing the database.");
        }
    }
}