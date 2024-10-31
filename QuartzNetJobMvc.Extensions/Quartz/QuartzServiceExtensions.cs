namespace QuartzNetJobMvc.Extensions.Quartz;

public static class QuartzServiceExtensions
{
    public static void ConfigureQuartz(this WebApplicationBuilder builder)
    {
        builder.Services.AddQuartz(q =>
        {
            q.ScheduleJob<EmailJob>(trigger => trigger
                .WithIdentity("emailJobTrigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(5)
                    .RepeatForever()));
        });

        builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }
}