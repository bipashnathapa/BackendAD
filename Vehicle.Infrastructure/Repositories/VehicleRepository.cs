using Microsoft.EntityFrameworkCore;
using Vehicle.Application.Interface.IRepositories;
using Vehicle.Domain.Models;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly ApplicationDbContext _context;

    public VehicleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddVehicleAsync(VehicleInfo vehicle)
    {
        await _context.Vehicles.AddAsync(vehicle);
        await _context.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<VehicleInfo>> GetVehiclesByCustomerIdAsync(int customerId)
    {
        return await _context.Vehicles
            .Where(v => v.CustomerID == customerId)
            .ToListAsync();
    }
}