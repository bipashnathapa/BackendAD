using System.Collections.Concurrent;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Vehicle.Application.Interface.IServices;

namespace Vehicle.Infrastructure.Service;

public class EmailOtpService : IEmailOtpService
{
    private const int MaxAttempts = 5;
    private static readonly TimeSpan Expiry = TimeSpan.FromMinutes(10);
    private static readonly ConcurrentDictionary<string, OtpEntry> Otps = new();

    private readonly IEmailService _emailService;

    public EmailOtpService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task SendOtpAsync(string email, string purpose, string displayName, CancellationToken cancellationToken = default)
    {
        var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        var key = BuildKey(email, purpose);

        Otps[key] = new OtpEntry
        {
            CodeHash = HashCode(code),
            ExpiresAt = DateTimeOffset.UtcNow.Add(Expiry)
        };

        var safeName = WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(displayName) ? "there" : displayName.Trim());
        var subject = "Your Vehicle Management OTP code";
        var html = $"""
            <div style="font-family:Arial,sans-serif;line-height:1.5;color:#111827">
                <p>Hello {safeName},</p>
                <p>Your OTP code is:</p>
                <p style="font-size:28px;font-weight:700;letter-spacing:4px;margin:16px 0">{code}</p>
                <p>This code expires in 10 minutes. If you did not request this, you can ignore this email.</p>
            </div>
            """;
        var sent = await _emailService.SendAsync(email, subject, html);
        if (!sent)
        {
            Otps.TryRemove(key, out _);
            throw new InvalidOperationException("Unable to send OTP email. Check SMTP configuration.");
        }
    }

    public bool VerifyOtp(string email, string purpose, string otpCode)
    {
        var key = BuildKey(email, purpose);
        if (!Otps.TryGetValue(key, out var entry)) return false;

        if (DateTimeOffset.UtcNow > entry.ExpiresAt || entry.Attempts >= MaxAttempts)
        {
            Otps.TryRemove(key, out _);
            return false;
        }

        if (SlowEquals(entry.CodeHash, HashCode(otpCode.Trim())))
        {
            Otps.TryRemove(key, out _);
            return true;
        }

        entry.Attempts++;
        if (entry.Attempts >= MaxAttempts)
        {
            Otps.TryRemove(key, out _);
        }

        return false;
    }

    public void ClearOtp(string email, string purpose)
    {
        Otps.TryRemove(BuildKey(email, purpose), out _);
    }

    private static string BuildKey(string email, string purpose)
    {
        return $"{purpose.Trim().ToLowerInvariant()}:{email.Trim().ToUpperInvariant()}";
    }

    private static string HashCode(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }

    private static bool SlowEquals(string first, string second)
    {
        var firstBytes = Encoding.UTF8.GetBytes(first);
        var secondBytes = Encoding.UTF8.GetBytes(second);
        return CryptographicOperations.FixedTimeEquals(firstBytes, secondBytes);
    }

    private sealed class OtpEntry
    {
        public string CodeHash { get; set; } = string.Empty;
        public DateTimeOffset ExpiresAt { get; set; }
        public int Attempts { get; set; }
    }
}
