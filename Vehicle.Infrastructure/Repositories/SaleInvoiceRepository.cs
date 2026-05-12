using Microsoft.EntityFrameworkCore;
using Vehicle.Application.Interface.IRepositories;
using Vehicle.Domain.Models;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Repositories;

public class SaleInvoiceRepository : ISaleInvoiceRepository
{
    private readonly ApplicationDbContext _db;
    public SaleInvoiceRepository(ApplicationDbContext db) => _db = db;

    public async Task<SaleInvoice> AddAsync(SaleInvoice invoice)
    {
        _db.SaleInvoices.Add(invoice);
        await _db.SaveChangesAsync();
        return invoice;
    }

    public Task<SaleInvoice?> GetByIdAsync(int id) =>
        _db.SaleInvoices
            .Include(i => i.Items).ThenInclude(it => it.Part)
            .Include(i => i.Customer).ThenInclude(c => c.User)
            .Include(i => i.Staff).ThenInclude(s => s.User)
            .FirstOrDefaultAsync(i => i.InvoiceID == id);

    public async Task<IReadOnlyList<SaleInvoice>> GetUnpaidOlderThanAsync(DateTime cutoffUtc) =>
        await _db.SaleInvoices
            .Include(i => i.Customer).ThenInclude(c => c.User)
            .Where(i => i.AmountDue > 0 && i.InvoiceDate <= cutoffUtc)
            .ToListAsync();

    public async Task<bool> MarkReminderSentAsync(int invoiceId)
    {
        var inv = await _db.SaleInvoices.FindAsync(invoiceId);
        if (inv == null) return false;
        inv.LastReminderSentAt = DateTime.UtcNow;
        return await _db.SaveChangesAsync() > 0;
    }
}
