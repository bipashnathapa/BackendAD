using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IRepositories;
using Vehicle.Application.Interface.IServices;
using Vehicle.Domain.Models;

namespace Vehicle.Infrastructure.Service;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepo;

    public VehicleService(IVehicleRepository vehicleRepo)
    {
        _vehicleRepo = vehicleRepo;
    }

    public async Task<bool> RegisterVehicleAsync(VehicleDTO vehicleDto)
    {
        var vehicle = new VehicleInfo
        {
            VehicleNo = vehicleDto.VehicleNo,
            Brand = vehicleDto.Brand,
            Model = vehicleDto.Model,
            Type = vehicleDto.Type,
            CustomerID = vehicleDto.CustomerId
        };

        await _vehicleRepo.AddVehicleAsync(vehicle);
        return true;
    }
}

