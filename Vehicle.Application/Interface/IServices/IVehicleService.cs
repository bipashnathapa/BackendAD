using Vehicle.Application.DTOs;

namespace Vehicle.Application.Interface.IServices;

public interface IVehicleService
{
    Task<bool> RegisterVehicleAsync(VehicleDTO vehicleDto);
}