namespace QuartzNetJobMvc.Extensions.Quartz;

public class EmailJob : IJob
{
    private readonly AppDbContext _dbContext;
    private readonly IFluentEmailFactory _emailFactory;

    public EmailJob(AppDbContext dbContext, IFluentEmailFactory emailFactory)
    {
        _dbContext = dbContext;
        _emailFactory = emailFactory;
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
}