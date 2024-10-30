// Extensions/EmailServiceExtensions.cs
using FluentEmail.Smtp;
using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection;

public static class EmailServiceExtensions
{
    public static void ConfigureFluentEmail(this IServiceCollection services)
    {
        services
            .AddFluentEmail("noreply@yourdomain.com")
            .AddSmtpSender("smtp.gmail.com", 587, "fromEmail", "EmailAppPassword");
    }
}
