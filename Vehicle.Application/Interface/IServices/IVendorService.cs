using Vehicle.Application.DTOs;

namespace Vehicle.Application.Interface.IServices;

public interface IVendorService
{
    Task<IReadOnlyList<VendorDTO>> GetAllAsync();
    Task<VendorDTO?> GetByIdAsync(int id);
    Task<VendorDTO> CreateAsync(CreateVendorDTO dto);
    Task<bool> UpdateAsync(int id, UpdateVendorDTO dto);
    Task<bool> DeleteAsync(int id);
}
