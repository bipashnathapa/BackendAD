using Vehicle.Application.DTOs;

namespace Vehicle.Application.Interface.IServices;

public interface IPartService
{
    Task<IReadOnlyList<PartDTO>> GetAllAsync();
    Task<PartDTO?> GetByIdAsync(int id);
    Task<PartDTO> CreateAsync(CreatePartDTO dto);
    Task<IReadOnlyList<PartDTO>> GetLowStockAsync();
}
