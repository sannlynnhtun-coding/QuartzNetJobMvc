// Extensions/EmailServiceExtensions.cs

using System.Net;
using System.Net.Mail;

namespace QuartzNetJobMvc.Extensions.Email;

public static class EmailServiceExtensions
{
    public static void ConfigureFluentEmail(this WebApplicationBuilder builder)
    {
        // Bind EmailSettings from appsettings.json
        var emailSettings = new EmailSettings();
        builder.Configuration.GetSection("EmailSettings").Bind(emailSettings);

        // Set up FluentEmail with SMTP sender using bound configuration values
        builder.Services
            .AddFluentEmail(emailSettings.FromEmail)
            .AddSmtpSender(new SmtpClient(emailSettings.SmtpHost)
            {
                Port = emailSettings.SmtpPort,
                Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password),
                EnableSsl = true
            });
    }
}