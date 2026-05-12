using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IRepositories;
using Vehicle.Application.Interface.IServices;
using Vehicle.Domain.Models;

namespace Vehicle.Infrastructure.Service;

public class VendorService : IVendorService
{
    private readonly IVendorRepository _repo;
    public VendorService(IVendorRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<VendorDTO>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(Map).ToList();

    public async Task<VendorDTO?> GetByIdAsync(int id)
    {
        var v = await _repo.GetByIdAsync(id);
        return v == null ? null : Map(v);
    }

    public async Task<VendorDTO> CreateAsync(CreateVendorDTO dto)
    {
        var v = new Vendor
        {
            Name = dto.Name.Trim(),
            ContactPerson = dto.ContactPerson.Trim(),
            Phone = dto.Phone.Trim(),
            Email = dto.Email?.Trim(),
            Address = dto.Address?.Trim(),
            PanNumber = dto.PanNumber?.Trim(),
        };
        return Map(await _repo.AddAsync(v));
    }

    public async Task<bool> UpdateAsync(int id, UpdateVendorDTO dto)
    {
        var v = await _repo.GetByIdAsync(id);
        if (v == null) return false;
        v.Name = dto.Name.Trim();
        v.ContactPerson = dto.ContactPerson.Trim();
        v.Phone = dto.Phone.Trim();
        v.Email = dto.Email?.Trim();
        v.Address = dto.Address?.Trim();
        v.PanNumber = dto.PanNumber?.Trim();
        v.IsActive = dto.IsActive;
        return await _repo.UpdateAsync(v);
    }

    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

    private static VendorDTO Map(Vendor v) => new()
    {
        VendorID = v.VendorID,
        Name = v.Name,
        ContactPerson = v.ContactPerson,
        Phone = v.Phone,
        Email = v.Email,
        Address = v.Address,
        PanNumber = v.PanNumber,
        IsActive = v.IsActive,
        CreatedAt = v.CreatedAt
    };
}
