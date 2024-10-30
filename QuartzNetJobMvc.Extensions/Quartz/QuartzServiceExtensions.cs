namespace QuartzNetJobMvc.Extensions.Quartz;

public static class QuartzServiceExtensions
{
    public static void ConfigureQuartz(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.ScheduleJob<EmailJob>(trigger => trigger
                .WithIdentity("emailJobTrigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(5)
                    .RepeatForever()));
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }
}