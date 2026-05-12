using Vehicle.Domain.Models;

namespace Vehicle.Application.Interface.IRepositories;

public interface IPartRepository
{
    Task<IReadOnlyList<Part>> GetAllAsync();
    Task<Part?> GetByIdAsync(int id);
    Task<Part> AddAsync(Part part);
    Task<bool> UpdateStockAsync(int partId, int newStock);
    Task<IReadOnlyList<Part>> GetLowStockAsync();
}
