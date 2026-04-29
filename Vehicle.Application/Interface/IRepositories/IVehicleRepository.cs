using Vehicle.Domain.Models;

namespace Vehicle.Application.Interface.IRepositories;

public interface IVehicleRepository
{
    Task AddVehicleAsync(VehicleInfo vehicle);
    Task<IEnumerable<VehicleInfo>> GetVehiclesByCustomerIdAsync(int customerId);
} 
