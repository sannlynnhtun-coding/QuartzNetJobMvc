namespace QuartzNetJobMvc.Extensions.Quartz;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        try
        {
            // Seed initial service settings if not already present
            if (!context.ServiceSettings.Any())
            {
                context.ServiceSettings.Add(new ServiceSetting
                {
                    ServiceName = "EmailService",
                    IsEnabled = true
                });
                Log.Information("Email service settings seeded.");
            }

            // Check if EmailJob exists in ScheduleSettings
            if (!context.ScheduleSettings.Any(s => s.JobType == "EmailJob"))
            {
                context.ScheduleSettings.Add(new ScheduleSetting
                {
                    ServiceName = "EmailService",
                    JobType = "EmailJob",
                    CronExpression = "0 0 1 * * ? " // Adjust the cron expression as needed
                });
                Log.Information("EmailJob settings seeded in ScheduleSettings.");
            }

            // Save changes to the database
            context.SaveChanges();
            Log.Information("Database initialized.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error initializing the database.");
        }
    }
}
