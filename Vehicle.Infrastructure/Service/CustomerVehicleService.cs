using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IRepositories;
using Vehicle.Application.Interface.IServices;
using Vehicle.Domain.Models;
using System.Linq;

namespace Vehicle.Infrastructure.Service;

public class CustomerVehicleService : ICustomerVehicleService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IVehicleRepository _vehicleRepository;

    // Use ONE constructor for all dependencies
    public CustomerVehicleService(
        ICustomerRepository customerRepository, 
        IVehicleRepository vehicleRepository)
    {
        _customerRepository = customerRepository;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<bool> AddMyVehicleAsync(string userId, CustomerAddVehicleDTO dto)
    {
        var customer = await _customerRepository.GetByUserIdAsync(userId);
        if (customer == null) return false;

        var vehicle = new VehicleInfo
        {
            VehicleNo = dto.VehicleNo,
            Brand = dto.Brand,
            Model = dto.Model,
            Type = dto.Type,
            CustomerID = customer.CustomerID
        };

        await _vehicleRepository.AddVehicleAsync(vehicle);
        return true;
    }

    public async Task<IReadOnlyList<CustomerVehicleDto>> GetMyVehiclesAsync(string userId)
    {
        var customer = await _customerRepository.GetByUserIdAsync(userId);
        if (customer == null) return Array.Empty<CustomerVehicleDto>();

        var vehicles = await _vehicleRepository.GetVehiclesByCustomerIdAsync(customer.CustomerID);
        return vehicles.Select(v => new CustomerVehicleDto
        {
            VehicleId = v.VehicleID,
            VehicleNo = v.VehicleNo,
            Brand = v.Brand,
            Model = v.Model,
            Type = v.Type
        }).ToList();
    }
}
