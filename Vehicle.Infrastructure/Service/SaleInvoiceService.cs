using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;
using Vehicle.Domain.Models;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Service;

public class SaleInvoiceService : ISaleInvoiceService
{
    private readonly ApplicationDbContext _db;
    private readonly IEmailService _email;

    public SaleInvoiceService(ApplicationDbContext db, IEmailService email)
    {
        _db = db;
        _email = email;
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
            }

            // Auto-discount rule: if subtotal > 5000, apply at least 10% discount.
            // This keeps the UI simple (staff doesn't need to calculate it).
            var discountPercent = dto.DiscountPercent;
            if (subTotal > 5000m && discountPercent < 10m) discountPercent = 10m;

            var discount = Math.Round(subTotal * discountPercent / 100m, 2);
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

            var dtoOut = new SaleInvoiceDTO
            {
                InvoiceID = invoice.InvoiceID,
                InvoiceNumber = invoice.InvoiceNumber,
                CustomerID = customer.CustomerID,
                CustomerName = customer.User.FullName,
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
                Items = invoice.Items.Select(it => new SaleInvoiceItemDTO
                {
                    PartID = it.PartID,
                    PartName = it.PartNameSnapshot,
                    Quantity = it.Quantity,
                    UnitPrice = it.UnitPrice,
                    LineTotal = it.LineTotal
                }).ToList()
            };

            // Email the invoice to the customer (in dev this will log to console unless SMTP is configured).
            var to = customer.User.Email;
            if (!string.IsNullOrWhiteSpace(to))
            {
                var subject = $"Your invoice {invoice.InvoiceNumber}";
                var body = $@"
<div style=""font-family:system-ui,-apple-system,Segoe UI,Roboto,Arial,sans-serif;line-height:1.45"">
  <h2 style=""margin:0 0 8px"">Invoice {invoice.InvoiceNumber}</h2>
  <div style=""color:#6b7280;margin:0 0 14px"">Date: {invoice.InvoiceDate:yyyy-MM-dd HH:mm} (UTC)</div>
  <div style=""border:1px solid #e5e7eb;border-radius:10px;padding:14px"">
    <div><strong>Subtotal:</strong> {invoice.SubTotal:N2}</div>
    <div><strong>Discount:</strong> −{invoice.Discount:N2}</div>
    <div><strong>Tax:</strong> {invoice.Tax:N2}</div>
    <div style=""margin-top:8px;font-size:16px""><strong>Total:</strong> {invoice.TotalAmount:N2}</div>
    <div style=""margin-top:2px;color:#6b7280"">Paid: {invoice.AmountPaid:N2} • Due: {invoice.AmountDue:N2}</div>
  </div>
  <h3 style=""margin:18px 0 8px"">Items</h3>
  <table cellpadding=""0"" cellspacing=""0"" style=""width:100%;border-collapse:collapse"">
    <thead>
      <tr>
        <th align=""left"" style=""border-bottom:1px solid #e5e7eb;padding:8px 0"">Part</th>
        <th align=""right"" style=""border-bottom:1px solid #e5e7eb;padding:8px 0"">Qty</th>
        <th align=""right"" style=""border-bottom:1px solid #e5e7eb;padding:8px 0"">Price</th>
        <th align=""right"" style=""border-bottom:1px solid #e5e7eb;padding:8px 0"">Total</th>
      </tr>
    </thead>
    <tbody>
      {string.Join("", invoice.Items.Select(it => $@"
      <tr>
        <td style=""border-bottom:1px solid #f3f4f6;padding:8px 0"">{System.Net.WebUtility.HtmlEncode(it.PartNameSnapshot)}</td>
        <td align=""right"" style=""border-bottom:1px solid #f3f4f6;padding:8px 0"">{it.Quantity}</td>
        <td align=""right"" style=""border-bottom:1px solid #f3f4f6;padding:8px 0"">{it.UnitPrice:N2}</td>
        <td align=""right"" style=""border-bottom:1px solid #f3f4f6;padding:8px 0"">{it.LineTotal:N2}</td>
      </tr>
      "))}
    </tbody>
  </table>
  <div style=""color:#6b7280;margin-top:14px"">Thank you.</div>
</div>";

                await _email.SendAsync(to, subject, body);
            }

            return dtoOut;
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

        return new SaleInvoiceDTO
        {
            InvoiceID = inv.InvoiceID,
            InvoiceNumber = inv.InvoiceNumber,
            CustomerID = inv.CustomerID,
            CustomerName = inv.Customer.User.FullName,
            StaffName = inv.Staff.User.FullName,
            SubTotal = inv.SubTotal,
            Discount = inv.Discount,
            Tax = inv.Tax,
            TotalAmount = inv.TotalAmount,
            AmountPaid = inv.AmountPaid,
            AmountDue = inv.AmountDue,
            PaymentStatus = inv.PaymentStatus,
            PaymentMethod = inv.PaymentMethod,
            InvoiceDate = inv.InvoiceDate,
            Items = inv.Items.Select(it => new SaleInvoiceItemDTO
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
