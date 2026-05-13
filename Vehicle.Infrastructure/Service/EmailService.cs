using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vehicle.Application.Interface.IServices;

namespace Vehicle.Infrastructure.Service;

public class EmailService : IEmailService
{
    private readonly SmtpOptions _options;
    private readonly ILogger<EmailService> _log;

    public EmailService(IOptions<SmtpOptions> options, ILogger<EmailService> log)
    {
        _options = options.Value;
        _log = log;
    }

    public async Task<bool> SendAsync(string to, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(to))
        {
            _log.LogWarning("Email skipped because recipient address is empty.");
            return false;
        }

        if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.Host))
        {
            _log.LogInformation("[EmailService:DEV] To={To} Subject={Subject}\n{Body}", to, subject, body);
            return false;
        }

        var user = _options.User?.Trim();
        var pass = _options.Pass?.Trim();
        var from = (_options.From ?? user)?.Trim();
        if (string.IsNullOrWhiteSpace(user) ||
            string.IsNullOrWhiteSpace(pass) ||
            string.IsNullOrWhiteSpace(from))
        {
            _log.LogWarning("SMTP is enabled but credentials are incomplete. Email to {To} was not sent.", to);
            return false;
        }

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Credentials = new NetworkCredential(user, pass),
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        using var msg = new MailMessage
        {
            From = new MailAddress(from, _options.FromName ?? "Vehicle Management"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        msg.To.Add(new MailAddress(to));

        try
        {
            await client.SendMailAsync(msg);
            _log.LogInformation("Email sent to {To} with subject {Subject}", to, subject);
            return true;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to send email to {To} with subject {Subject}", to, subject);
            return false;
        }
    }
}
