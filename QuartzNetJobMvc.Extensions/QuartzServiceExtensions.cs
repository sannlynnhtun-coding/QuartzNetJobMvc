// Extensions/QuartzServiceExtensions.cs
using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Serilog;

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

public class EmailJob : IJob
{
    private readonly YourDbContext _dbContext;
    private readonly IFluentEmailFactory _emailFactory;

    public EmailJob(YourDbContext dbContext, IFluentEmailFactory emailFactory)
    {
        _dbContext = dbContext;
        _emailFactory = emailFactory;
    }

    private async Task SendAnnouncementEmailAsync(Announcement announcement)
    {
        try
        {
            var email = await _emailFactory
                .Create()
                .To("recipient@example.com") // Replace with the recipient's email address
                .Subject(announcement.Title)
                .Body(announcement.Message, true) // Use 'true' for HTML body
                .SendAsync();

            if (email.Successful)
            {
                Log.Information($"Email sent: {announcement.Title}");
            }
            else
            {
                Log.Error($"Failed to send email: {string.Join(", ", email.ErrorMessages)}");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error sending email for announcement: {Title}", announcement.Title);
        }
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            bool isEmailServiceEnabled = await _dbContext.ServiceSettings
                .Where(s => s.ServiceName == "EmailService")
                .Select(s => s.IsEnabled)
                .FirstOrDefaultAsync();

            if (isEmailServiceEnabled)
            {
                var announcements = await _dbContext.Announcements
                    .Where(a => !a.IsSent)
                    .ToListAsync();

                foreach (var announcement in announcements)
                {
                    await SendAnnouncementEmailAsync(announcement);

                    // Mark announcement as sent
                    announcement.IsSent = true;
                    _dbContext.Update(announcement);
                }

                await _dbContext.SaveChangesAsync();
            }
            else
            {
                Log.Warning("Email service is turned off.");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error executing EmailJob");
        }
    }
}


public class YourDbContext : DbContext
{
    public YourDbContext(DbContextOptions<YourDbContext> options) : base(options) { }

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

public class ServiceSetting
{
    public int Id { get; set; }
    public string ServiceName { get; set; }
    public bool IsEnabled { get; set; }
}

public class Announcement
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsSent { get; set; } = false;
}

public static class DbInitializer
{
    public static void Initialize(YourDbContext context)
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
                Id = 1,
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
