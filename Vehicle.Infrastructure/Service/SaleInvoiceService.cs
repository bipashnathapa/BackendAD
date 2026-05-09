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
    public SaleInvoiceService(ApplicationDbContext db) => _db = db;

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

            return new SaleInvoiceDTO
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
