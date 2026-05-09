using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vehicle.Application.Interface.IServices;
using Vehicle.Domain.Models;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Service;

// Periodically:
//   - notifies all admins by email when any active part has stock < threshold
//   - emails customers with credit invoices unpaid > 30 days (max once per 7 days)
public class NotificationWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<NotificationWorker> _log;
    private static readonly TimeSpan Interval = TimeSpan.FromHours(6);
    private const int CreditOverdueDays = 30;
    private const int ReminderCooldownDays = 7;

    public NotificationWorker(IServiceProvider services, ILogger<NotificationWorker> log)
    {
        _services = services;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // small startup delay so the host finishes wiring up
        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunOnceAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "NotificationWorker iteration failed");
            }

            try { await Task.Delay(Interval, stoppingToken); }
            catch (TaskCanceledException) { break; }
        }
    }

    private async Task RunOnceAsync(CancellationToken ct)
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var email = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // ---------- Low stock -> email all admins ----------
        var lowStock = await db.Parts
            .Include(p => p.Vendor)
            .Where(p => p.IsActive && p.StockQuantity < p.LowStockThreshold)
            .ToListAsync(ct);

        if (lowStock.Count > 0)
        {
            var admins = await userMgr.GetUsersInRoleAsync("Admin");
            if (admins.Count == 0)
                _log.LogWarning("Low stock detected but no Admin users to notify.");

            var rows = string.Join("", lowStock.Select(p =>
                $"<tr><td>{p.PartName}</td><td>{p.PartCode}</td><td>{p.StockQuantity}</td><td>{p.LowStockThreshold}</td><td>{p.Vendor?.Name ?? "-"}</td></tr>"));

            var html = $@"<h3>Low Stock Alert ({lowStock.Count} parts)</h3>
<table border='1' cellpadding='6' cellspacing='0'>
<tr><th>Part</th><th>Code</th><th>Stock</th><th>Threshold</th><th>Vendor</th></tr>
{rows}
</table>";

            foreach (var admin in admins)
            {
                if (string.IsNullOrWhiteSpace(admin.Email)) continue;
                await email.SendAsync(admin.Email, "[VMS] Low stock alert", html);
            }
        }

        // ---------- Pending credits > 30 days -> email customer ----------
        var cutoff = DateTime.UtcNow.AddDays(-CreditOverdueDays);
        var reminderCutoff = DateTime.UtcNow.AddDays(-ReminderCooldownDays);

        var overdue = await db.SaleInvoices
            .Include(i => i.Customer).ThenInclude(c => c.User)
            .Where(i => i.AmountDue > 0
                     && i.InvoiceDate <= cutoff
                     && (i.LastReminderSentAt == null || i.LastReminderSentAt <= reminderCutoff))
            .ToListAsync(ct);

        foreach (var inv in overdue)
        {
            var to = inv.Customer.User.Email;
            if (string.IsNullOrWhiteSpace(to)) continue;

            var days = (int)(DateTime.UtcNow - inv.InvoiceDate).TotalDays;
            var body = $@"<p>Dear {inv.Customer.User.FullName},</p>
<p>Our records show invoice <b>{inv.InvoiceNumber}</b> dated <b>{inv.InvoiceDate:yyyy-MM-dd}</b> has a pending balance of <b>NPR {inv.AmountDue:N2}</b> outstanding for {days} days.</p>
<p>Please settle the amount at your earliest convenience.</p>
<p>Thank you,<br/>Vehicle Management</p>";

            await email.SendAsync(to, $"[VMS] Payment reminder — {inv.InvoiceNumber}", body);
            inv.LastReminderSentAt = DateTime.UtcNow;
        }

        if (overdue.Count > 0)
            await db.SaveChangesAsync(ct);

        _log.LogInformation("NotificationWorker run: lowStock={Low}, overdueReminders={Over}", lowStock.Count, overdue.Count);
    }
}
