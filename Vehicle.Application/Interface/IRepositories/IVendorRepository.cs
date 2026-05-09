using Vehicle.Domain.Models;

namespace Vehicle.Application.Interface.IRepositories;

public interface IVendorRepository
{
    Task<IReadOnlyList<Vendor>> GetAllAsync();
    Task<Vendor?> GetByIdAsync(int id);
    Task<Vendor> AddAsync(Vendor vendor);
    Task<bool> UpdateAsync(Vendor vendor);
    Task<bool> DeleteAsync(int id);
}
