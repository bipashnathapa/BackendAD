using Vehicle.Domain.Models;

namespace Vehicle.Application.Interface.IRepositories;

public interface ISaleInvoiceRepository
{
    Task<SaleInvoice> AddAsync(SaleInvoice invoice);
    Task<SaleInvoice?> GetByIdAsync(int id);
    Task<IReadOnlyList<SaleInvoice>> GetUnpaidOlderThanAsync(DateTime cutoffUtc);
    Task<bool> MarkReminderSentAsync(int invoiceId);
}
