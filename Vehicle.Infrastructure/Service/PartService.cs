using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IRepositories;
using Vehicle.Application.Interface.IServices;
using Vehicle.Domain.Models;

namespace Vehicle.Infrastructure.Service;

public class PartService : IPartService
{
    private readonly IPartRepository _repo;
    public PartService(IPartRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<PartDTO>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(Map).ToList();

    public async Task<PartDTO?> GetByIdAsync(int id)
    {
        var p = await _repo.GetByIdAsync(id);
        return p == null ? null : Map(p);
    }

    public async Task<PartDTO> CreateAsync(CreatePartDTO dto)
    {
        var p = new Part
        {
            PartName = dto.PartName.Trim(),
            PartCode = dto.PartCode?.Trim(),
            Compatibility = dto.Compatibility?.Trim(),
            UnitPrice = dto.UnitPrice,
            StockQuantity = dto.StockQuantity,
            LowStockThreshold = dto.LowStockThreshold,
            VendorID = dto.VendorID
        };
        return Map(await _repo.AddAsync(p));
    }

    public async Task<IReadOnlyList<PartDTO>> GetLowStockAsync() =>
        (await _repo.GetLowStockAsync()).Select(Map).ToList();

    private static PartDTO Map(Part p) => new()
    {
        PartID = p.PartID,
        PartName = p.PartName,
        PartCode = p.PartCode,
        Compatibility = p.Compatibility,
        UnitPrice = p.UnitPrice,
        StockQuantity = p.StockQuantity,
        LowStockThreshold = p.LowStockThreshold,
        VendorID = p.VendorID,
        VendorName = p.Vendor?.Name,
        IsActive = p.IsActive
    };
}
