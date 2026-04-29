using Vehicle.Application.DTOs;
using Vehicle.Domain.Models;

namespace Vehicle.Application.Interface.IServices;

public interface ICustomerVehicleService
{
    Task<bool> AddMyVehicleAsync(string userId, CustomerAddVehicleDTO dto);
    Task<IReadOnlyList<CustomerVehicleDto>> GetMyVehiclesAsync(string userId);
}
