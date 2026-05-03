using Microsoft.EntityFrameworkCore;
using Vehicle.Application.Interface.IRepositories;
using Vehicle.Domain.Models;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Repositories;

public class CustomerServiceRepository : ICustomerServiceRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerServiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment> AddAppointmentAsync(Appointment appointment)
    {
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }

    public async Task<IReadOnlyList<Appointment>> GetAppointmentsByCustomerIdAsync(int customerId)
    {
        return await _context.Appointments
            .Include(a => a.Vehicle)
            .Where(a => a.CustomerID == customerId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<PartRequest> AddPartRequestAsync(PartRequest partRequest)
    {
        await _context.PartRequests.AddAsync(partRequest);
        await _context.SaveChangesAsync();
        return partRequest;
    }

    public async Task<IReadOnlyList<PartRequest>> GetPartRequestsByCustomerIdAsync(int customerId)
    {
        return await _context.PartRequests
            .Include(p => p.Vehicle)
            .Where(p => p.CustomerID == customerId)
            .OrderByDescending(p => p.RequestedAt)
            .ToListAsync();
    }

    public async Task<ServiceReview> AddServiceReviewAsync(ServiceReview review)
    {
        await _context.ServiceReviews.AddAsync(review);
        await _context.SaveChangesAsync();
        return review;
    }

    public async Task<IReadOnlyList<ServiceReview>> GetServiceReviewsByCustomerIdAsync(int customerId)
    {
        return await _context.ServiceReviews
            .Where(r => r.CustomerID == customerId)
            .OrderByDescending(r => r.ReviewedAt)
            .ToListAsync();
    }

    public async Task<bool> CustomerOwnsVehicleAsync(int customerId, int vehicleId)
    {
        return await _context.Vehicles.AnyAsync(v => v.CustomerID == customerId && v.VehicleID == vehicleId);
    }

    public async Task<bool> CustomerOwnsAppointmentAsync(int customerId, int appointmentId)
    {
        return await _context.Appointments.AnyAsync(a => a.CustomerID == customerId && a.AppointmentID == appointmentId);
    }
}
