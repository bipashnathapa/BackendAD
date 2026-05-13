namespace Vehicle.Infrastructure.Service;

public class SmtpOptions
{
    public const string SectionName = "Smtp";

    public bool Enabled { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; } = 587;
    public string? User { get; set; }
    public string? Pass { get; set; }
    public string? From { get; set; }
    public string? FromName { get; set; } = "Vehicle Management";
    public bool EnableSsl { get; set; } = true;
}
