using Vehicle.Application.DTOs;

namespace Vehicle.Application.Interface.IServices;

public interface IAdminStaffService
{
    Task<(bool Ok, string Message)> CreateStaffAsync(AdminCreateStaffDto dto);
    Task<IReadOnlyList<AdminStaffListItemDto>> GetStaffAsync(int take = 100);
}

