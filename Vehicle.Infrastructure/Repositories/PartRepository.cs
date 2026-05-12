using Microsoft.EntityFrameworkCore;
using Vehicle.Application.Interface.IRepositories;
using Vehicle.Domain.Models;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Repositories;

public class PartRepository : IPartRepository
{
    private readonly ApplicationDbContext _db;
    public PartRepository(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<Part>> GetAllAsync() =>
        await _db.Parts.Include(p => p.Vendor).OrderBy(p => p.PartName).ToListAsync();

    public Task<Part?> GetByIdAsync(int id) =>
        _db.Parts.Include(p => p.Vendor).FirstOrDefaultAsync(p => p.PartID == id);

    public async Task<Part> AddAsync(Part part)
    {
        _db.Parts.Add(part);
        await _db.SaveChangesAsync();
        return part;
    }

    public async Task<bool> UpdateStockAsync(int partId, int newStock)
    {
        var p = await _db.Parts.FindAsync(partId);
        if (p == null) return false;
        p.StockQuantity = newStock;
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<IReadOnlyList<Part>> GetLowStockAsync() =>
        await _db.Parts
            .Include(p => p.Vendor)
            .Where(p => p.IsActive && p.StockQuantity < p.LowStockThreshold)
            .ToListAsync();
}
