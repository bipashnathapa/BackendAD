using Microsoft.EntityFrameworkCore;
using Vehicle.Application.Interface.IRepositories;
using Vehicle.Domain.Models;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Repositories;

public class VendorRepository : IVendorRepository
{
    private readonly ApplicationDbContext _db;
    public VendorRepository(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<Vendor>> GetAllAsync() =>
        await _db.Vendors.OrderByDescending(v => v.CreatedAt).ToListAsync();

    public Task<Vendor?> GetByIdAsync(int id) =>
        _db.Vendors.FirstOrDefaultAsync(v => v.VendorID == id);

    public async Task<Vendor> AddAsync(Vendor vendor)
    {
        _db.Vendors.Add(vendor);
        await _db.SaveChangesAsync();
        return vendor;
    }

    public async Task<bool> UpdateAsync(Vendor vendor)
    {
        vendor.UpdatedAt = DateTime.UtcNow;
        _db.Vendors.Update(vendor);
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var v = await _db.Vendors.FindAsync(id);
        if (v == null) return false;
        v.IsActive = false; // soft delete to preserve invoice history
        v.UpdatedAt = DateTime.UtcNow;
        return await _db.SaveChangesAsync() > 0;
    }
}
