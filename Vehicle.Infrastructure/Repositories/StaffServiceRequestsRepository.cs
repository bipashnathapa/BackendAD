using Microsoft.EntityFrameworkCore;
using Vehicle.Application.Interface.IRepositories;
using Vehicle.Domain.Models;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Repositories;

public class StaffServiceRequestsRepository : IStaffServiceRequestsRepository
{
    private readonly ApplicationDbContext _db;

    public StaffServiceRequestsRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Appointment>> GetAppointmentsAsync(string? status, int take)
    {
        var q = _db.Appointments
            .AsNoTracking()
            .Include(a => a.Vehicle)
            .Include(a => a.Customer)
            .ThenInclude(c => c.User)
            .OrderByDescending(a => a.CreatedAt)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            q = q.Where(a => a.Status == status);
        }

        take = Math.Clamp(take, 1, 200);
        return await q.Take(take).ToListAsync();
    }

    public Task<Appointment?> GetAppointmentByIdAsync(int id)
        => _db.Appointments.Include(a => a.Customer).ThenInclude(c => c.User).FirstOrDefaultAsync(a => a.AppointmentID == id);

    public async Task<IReadOnlyList<PartRequest>> GetPartRequestsAsync(string? status, int take)
    {
        var q = _db.PartRequests
            .AsNoTracking()
            .Include(p => p.Vehicle)
            .Include(p => p.Customer)
            .ThenInclude(c => c.User)
            .OrderByDescending(p => p.RequestedAt)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            q = q.Where(p => p.Status == status);
        }

        take = Math.Clamp(take, 1, 200);
        return await q.Take(take).ToListAsync();
    }

    public Task<PartRequest?> GetPartRequestByIdAsync(int id)
        => _db.PartRequests.Include(p => p.Customer).ThenInclude(c => c.User).FirstOrDefaultAsync(p => p.PartRequestID == id);

    public async Task<IReadOnlyList<ServiceReview>> GetServiceReviewsAsync(int take)
    {
        take = Math.Clamp(take, 1, 200);
        return await _db.ServiceReviews
            .AsNoTracking()
            .Include(r => r.Customer)
            .ThenInclude(c => c.User)
            .OrderByDescending(r => r.ReviewedAt)
            .Take(take)
            .ToListAsync();
    }

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}

