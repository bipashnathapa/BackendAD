using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Net;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;
using Vehicle.Domain.Models;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Service;

public class SaleInvoiceService : ISaleInvoiceService
{
    private readonly ApplicationDbContext _db;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public SaleInvoiceService(ApplicationDbContext db, IEmailService emailService, IConfiguration configuration)
    {
        _db = db;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<SaleInvoiceDTO?> CreateAsync(string staffUserId, CreateSaleInvoiceDTO dto)
    {
        var staff = await _db.Staffs.Include(s => s.User).FirstOrDefaultAsync(s => s.UserID == staffUserId);
        if (staff == null) return null;

        var customer = await _db.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.CustomerID == dto.CustomerID);
        if (customer == null) return null;

        // Lock parts and validate stock atomically
        await using IDbContextTransaction tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var partIds = dto.Items.Select(i => i.PartID).Distinct().ToList();
            var parts = await _db.Parts.Where(p => partIds.Contains(p.PartID)).ToListAsync();
            if (parts.Count != partIds.Count) return null;

            var items = new List<SaleInvoiceItem>();
            var lowStockParts = new List<Part>();
            decimal subTotal = 0m;

            foreach (var line in dto.Items)
            {
                var part = parts.First(p => p.PartID == line.PartID);
                if (part.StockQuantity < line.Quantity || !part.IsActive) return null;

                var lineTotal = part.UnitPrice * line.Quantity;
                subTotal += lineTotal;

                items.Add(new SaleInvoiceItem
                {
                    PartID = part.PartID,
                    PartNameSnapshot = part.PartName,
                    Quantity = line.Quantity,
                    UnitPrice = part.UnitPrice,
                    LineTotal = lineTotal
                });

                part.StockQuantity -= line.Quantity;
                if (part.StockQuantity <= part.LowStockThreshold)
                {
                    lowStockParts.Add(part);
                }
            }

            var discount = Math.Round(subTotal * dto.DiscountPercent / 100m, 2);
            var taxBase = subTotal - discount;
            var tax = Math.Round(taxBase * dto.TaxPercent / 100m, 2);
            var total = Math.Round(taxBase + tax, 2);
            var paid = Math.Min(dto.AmountPaid, total);
            var due = Math.Round(total - paid, 2);

            var status = due <= 0 ? "Paid" : (paid > 0 ? "PartiallyPaid" : "Credit");

            var invoice = new SaleInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
                CustomerID = customer.CustomerID,
                StaffID = staff.StaffID,
                SubTotal = subTotal,
                Discount = discount,
                Tax = tax,
                TotalAmount = total,
                AmountPaid = paid,
                AmountDue = due,
                PaymentStatus = status,
                PaymentMethod = dto.PaymentMethod,
                Notes = dto.Notes,
                PaidAt = due <= 0 ? DateTime.UtcNow : null,
                Items = items
            };

            _db.SaleInvoices.Add(invoice);
            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            var invoiceEmailSent = await TrySendInvoiceEmailAsync(invoice, customer, staff);
            var lowStockAlertSent = await TrySendLowStockAlertAsync(lowStockParts);

            return Map(invoice, customer, staff, invoiceEmailSent, lowStockAlertSent);
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<SaleInvoiceDTO?> GetByIdAsync(int id)
    {
        var inv = await _db.SaleInvoices
            .Include(i => i.Items)
            .Include(i => i.Customer).ThenInclude(c => c.User)
            .Include(i => i.Staff).ThenInclude(s => s.User)
            .FirstOrDefaultAsync(i => i.InvoiceID == id);

        if (inv == null) return null;

        return Map(inv, inv.Customer, inv.Staff, false, false);
    }

    private async Task<bool> TrySendInvoiceEmailAsync(SaleInvoice invoice, Customer customer, Staff staff)
    {
        var to = customer.User.Email;
        if (string.IsNullOrWhiteSpace(to)) return false;

        try
        {
            await _emailService.SendAsync(to, $"Invoice {invoice.InvoiceNumber}", BuildInvoiceEmail(invoice, customer, staff));
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> TrySendLowStockAlertAsync(IReadOnlyCollection<Part> parts)
    {
        var lowStockParts = parts
            .Where(p => p.StockQuantity <= p.LowStockThreshold)
            .GroupBy(p => p.PartID)
            .Select(g => g.First())
            .ToList();

        if (lowStockParts.Count == 0) return false;

        var configuredRecipients = (_configuration["Inventory:LowStockAlertEmails"] ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var userRecipients = await _db.Users
            .Where(u => (u.UserRole == "Staff" || u.UserRole == "Admin") && u.Email != null)
            .Select(u => u.Email!)
            .ToListAsync();

        var recipients = configuredRecipients
            .Concat(userRecipients)
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (recipients.Count == 0) return false;

        var rows = string.Join("", lowStockParts.Select(p =>
            $"<tr><td>{WebUtility.HtmlEncode(p.PartName)}</td><td>{WebUtility.HtmlEncode(p.PartCode ?? "-")}</td><td>{p.StockQuantity}</td><td>{p.LowStockThreshold}</td></tr>"));

        var html = $"""
            <h3>Low stock alert</h3>
            <p>The following items are at or below their configured low-stock threshold.</p>
            <table border="1" cellpadding="6" cellspacing="0">
                <tr><th>Part</th><th>Code</th><th>Stock</th><th>Threshold</th></tr>
                {rows}
            </table>
            """;

        try
        {
            foreach (var recipient in recipients)
            {
                await _emailService.SendAsync(recipient, "[VMS] Low stock alert", html);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string BuildInvoiceEmail(SaleInvoice invoice, Customer customer, Staff staff)
    {
        var rows = string.Join("", invoice.Items.Select(i =>
            $"""
            <tr>
                <td>{WebUtility.HtmlEncode(i.PartNameSnapshot)}</td>
                <td style="text-align:right">{i.Quantity}</td>
                <td style="text-align:right">NPR {i.UnitPrice:N2}</td>
                <td style="text-align:right">NPR {i.LineTotal:N2}</td>
            </tr>
            """));

        var notes = string.IsNullOrWhiteSpace(invoice.Notes)
            ? string.Empty
            : $"<p><strong>Notes:</strong> {WebUtility.HtmlEncode(invoice.Notes)}</p>";

        return $"""
            <div style="font-family:Arial,sans-serif;line-height:1.5;color:#111827">
                <h2>Invoice {WebUtility.HtmlEncode(invoice.InvoiceNumber)}</h2>
                <p>Hello {WebUtility.HtmlEncode(customer.User.FullName)},</p>
                <p>Your invoice has been prepared by {WebUtility.HtmlEncode(staff.User.FullName)}.</p>
                <table style="border-collapse:collapse;width:100%" cellpadding="8">
                    <thead>
                        <tr>
                            <th style="text-align:left;border-bottom:2px solid #111827">Part</th>
                            <th style="text-align:right;border-bottom:2px solid #111827">Qty</th>
                            <th style="text-align:right;border-bottom:2px solid #111827">Unit</th>
                            <th style="text-align:right;border-bottom:2px solid #111827">Total</th>
                        </tr>
                    </thead>
                    <tbody>{rows}</tbody>
                </table>
                <p><strong>Subtotal:</strong> NPR {invoice.SubTotal:N2}</p>
                <p><strong>Discount:</strong> NPR {invoice.Discount:N2}</p>
                <p><strong>Tax:</strong> NPR {invoice.Tax:N2}</p>
                <p><strong>Total:</strong> NPR {invoice.TotalAmount:N2}</p>
                <p><strong>Paid:</strong> NPR {invoice.AmountPaid:N2}</p>
                <p><strong>Due:</strong> NPR {invoice.AmountDue:N2}</p>
                {notes}
                <p>Thank you.</p>
            </div>
            """;
    }

    private static SaleInvoiceDTO Map(
        SaleInvoice invoice,
        Customer customer,
        Staff staff,
        bool invoiceEmailSent,
        bool lowStockAlertSent)
    {
        return new SaleInvoiceDTO
        {
            InvoiceID = invoice.InvoiceID,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerID = invoice.CustomerID,
            CustomerName = customer.User.FullName,
            CustomerEmail = customer.User.Email,
            StaffName = staff.User.FullName,
            SubTotal = invoice.SubTotal,
            Discount = invoice.Discount,
            Tax = invoice.Tax,
            TotalAmount = invoice.TotalAmount,
            AmountPaid = invoice.AmountPaid,
            AmountDue = invoice.AmountDue,
            PaymentStatus = invoice.PaymentStatus,
            PaymentMethod = invoice.PaymentMethod,
            InvoiceDate = invoice.InvoiceDate,
            InvoiceEmailSent = invoiceEmailSent,
            LowStockAlertSent = lowStockAlertSent,
            Items = invoice.Items.Select(it => new SaleInvoiceItemDTO
            {
                PartID = it.PartID,
                PartName = it.PartNameSnapshot,
                Quantity = it.Quantity,
                UnitPrice = it.UnitPrice,
                LineTotal = it.LineTotal
            }).ToList()
        };
    }
}
