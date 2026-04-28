using Vehicle.Domain.Models;

namespace Vehicle.Application.Interface.IRepositories;

public interface IVehicleRepository
{
    Task AddVehicleAsync(VehicleInfo vehicle);
}