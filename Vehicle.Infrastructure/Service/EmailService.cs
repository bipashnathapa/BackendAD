using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using Vehicle.Application.Interface.IServices;

namespace Vehicle.Infrastructure.Service;

// Sends mail via SMTP if configured; otherwise logs the message.
// Configure under "Smtp": { "Host", "Port", "User", "Pass", "From", "Enabled": true }
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
        var user = _cfg["Smtp:User"];
        var pass = _cfg["Smtp:Pass"];
        var from = _cfg["Smtp:From"] ?? user ?? "no-reply@localhost";

        using var client = new SmtpClient(host, port) { EnableSsl = true };
        if (!string.IsNullOrWhiteSpace(user))
            client.Credentials = new NetworkCredential(user, pass);

        var msg = new MailMessage(from, to, subject, body) { IsBodyHtml = true };
        try
        {
            await client.SendMailAsync(msg);
            _log.LogInformation("Email sent to {To}", to);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to send email to {To}", to);
        }
    }
}
