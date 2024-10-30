using FluentEmail.Smtp;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Net.Mail;
using System.Net;
using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure MSSqlServerSinkOptions
var sinkOptions = new MSSqlServerSinkOptions
{
    TableName = "Logs",              // Specify the table name
    AutoCreateSqlTable = true         // Automatically create the table if it doesn't exist
};

// Define the column options (optional)
var columnOptions = new ColumnOptions
{
    AdditionalColumns = new[]
    {
        new SqlColumn { ColumnName = "CustomColumn1", DataType = SqlDbType.NVarChar, DataLength = 100 }
    }
};

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.MSSqlServer(
        connectionString: "YourConnectionString",  // Your SQL Server connection string
        sinkOptions: sinkOptions,                  // Use the new sinkOptions object
        columnOptions: columnOptions)              // Optional column options
    .CreateLogger();

builder.Host.UseSerilog(); // Use Serilog for logging

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure FluentEmail
builder.Services
    .AddFluentEmail("noreply@yourdomain.com")
    .AddSmtpSender("smtp.gmail.com", 587, "fromEmail", "Email App Password");

builder.Services.AddQuartz(q =>
{
    // Schedule the EmailJob
    q.ScheduleJob<EmailJob>(trigger => trigger
        .WithIdentity("emailJobTrigger")
        .StartNow()
        .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(5)
            .RepeatForever()));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddDbContext<YourDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<YourDbContext>();
    DbInitializer.Initialize(context);
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

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

