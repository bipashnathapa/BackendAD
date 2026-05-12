using Microsoft.EntityFrameworkCore;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IRepositories;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _db;
    public ReportRepository(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<RegularCustomerDTO>> GetRegularCustomersAsync(int minVisits, int days)
    {
        var since = DateTime.UtcNow.AddDays(-days);
        return await _db.SaleInvoices
            .Where(i => i.InvoiceDate >= since)
            .GroupBy(i => i.CustomerID)
            .Select(g => new
            {
                CustomerID = g.Key,
                Visits = g.Count(),
                LastVisit = g.Max(x => x.InvoiceDate)
            })
            .Where(x => x.Visits >= minVisits)
            .Join(_db.Customers.Include(c => c.User),
                stat => stat.CustomerID,
                cust => cust.CustomerID,
                (stat, cust) => new RegularCustomerDTO
                {
                    CustomerID = cust.CustomerID,
                    FullName = cust.User.FullName,
                    Email = cust.User.Email,
                    VisitCount = stat.Visits,
                    LastVisit = stat.LastVisit
                })
            .OrderByDescending(r => r.VisitCount)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<HighSpenderDTO>> GetHighSpendersAsync(int top)
    {
        return await _db.SaleInvoices
            .GroupBy(i => i.CustomerID)
            .Select(g => new
            {
                CustomerID = g.Key,
                TotalSpent = g.Sum(x => x.TotalAmount),
                InvoiceCount = g.Count()
            })
            .Join(_db.Customers.Include(c => c.User),
                stat => stat.CustomerID,
                cust => cust.CustomerID,
                (stat, cust) => new HighSpenderDTO
                {
                    CustomerID = cust.CustomerID,
                    FullName = cust.User.FullName,
                    Email = cust.User.Email,
                    TotalSpent = stat.TotalSpent,
                    InvoiceCount = stat.InvoiceCount
                })
            .OrderByDescending(r => r.TotalSpent)
            .Take(top)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<PendingCreditDTO>> GetPendingCreditsAsync(int olderThanDays)
    {
        var cutoff = DateTime.UtcNow.AddDays(-olderThanDays);
        var raw = await _db.SaleInvoices
            .Include(i => i.Customer).ThenInclude(c => c.User)
            .Where(i => i.AmountDue > 0 && i.InvoiceDate <= cutoff)
            .ToListAsync();

        return raw
            .GroupBy(i => i.CustomerID)
            .Select(g => new PendingCreditDTO
            {
                CustomerID = g.Key,
                FullName = g.First().Customer.User.FullName,
                Email = g.First().Customer.User.Email,
                Phone = g.First().Customer.User.PhoneNumber,
                TotalDue = g.Sum(i => i.AmountDue),
                OverdueInvoiceCount = g.Count(),
                OldestUnpaidDate = g.Min(i => i.InvoiceDate),
                DaysOverdue = (int)(DateTime.UtcNow - g.Min(i => i.InvoiceDate)).TotalDays
            })
            .OrderByDescending(r => r.TotalDue)
            .ToList();
    }
}
