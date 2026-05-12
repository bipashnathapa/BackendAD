using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using Vehicle.Application.Interface.IServices;

namespace Vehicle.Infrastructure.Service;

// Sends mail via SMTP if configured; otherwise logs the message.
// Configure under "Smtp": { "Host", "Port", "User", "Pass", "From", "FromName", "Enabled": true }
public class EmailService : IEmailService
{
    private readonly IConfiguration _cfg;
    private readonly ILogger<EmailService> _log;

    public EmailService(IConfiguration cfg, ILogger<EmailService> log)
    {
        _cfg = cfg;
        _log = log;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var enabled = _cfg.GetValue<bool>("Smtp:Enabled");
        if (!enabled || string.IsNullOrWhiteSpace(_cfg["Smtp:Host"]))
        {
            _log.LogInformation("[EmailService:DEV] To={To} Subject={Subject}\n{Body}", to, subject, body);
            return;
        }

        var host = _cfg["Smtp:Host"]!;
        var port = _cfg.GetValue<int>("Smtp:Port", 587);
        var enableSsl = _cfg.GetValue("Smtp:EnableSsl", true);
        var user = _cfg["Smtp:User"];
        var pass = _cfg["Smtp:Pass"];
        var from = _cfg["Smtp:From"] ?? user;
        var fromName = _cfg["Smtp:FromName"] ?? "Vehicle Management";

        if (string.IsNullOrWhiteSpace(user) ||
            string.IsNullOrWhiteSpace(pass) ||
            string.IsNullOrWhiteSpace(from))
        {
            throw new InvalidOperationException("SMTP credentials are not configured.");
        }

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            Credentials = new NetworkCredential(user, pass),
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        using var msg = new MailMessage
        {
            From = new MailAddress(from, fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        msg.To.Add(new MailAddress(to));
        try
        {
            await client.SendMailAsync(msg);
            _log.LogInformation("Email sent to {To}", to);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }
}
