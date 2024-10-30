// Extensions/EmailServiceExtensions.cs
using FluentEmail.Smtp;
using FluentEmail.Core;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class EmailServiceExtensions
{
    public static void ConfigureFluentEmail(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind EmailSettings from appsettings.json
        var emailSettings = new EmailSettings();
        configuration.GetSection("EmailSettings").Bind(emailSettings);

        // Set up FluentEmail with SMTP sender using bound configuration values
        services
            .AddFluentEmail(emailSettings.FromEmail)
            .AddSmtpSender(new SmtpClient(emailSettings.SmtpHost)
            {
                Port = emailSettings.SmtpPort,
                Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password),
                EnableSsl = true
            });
    }
}

public class EmailSettings
{
    public string FromEmail { get; set; }
    public string SmtpHost { get; set; }
    public int SmtpPort { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
