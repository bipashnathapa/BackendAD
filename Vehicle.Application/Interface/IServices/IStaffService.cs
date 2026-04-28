using Microsoft.AspNetCore.Identity;
using Vehicle.Application.DTOs;

namespace Vehicle.Application.Interface.IServices;

public interface IStaffService
{
    Task<(IdentityResult Result, StaffRegisterCustomerResultDto? Data)> RegisterCustomerWithVehicleAsync(StaffRegisterCustomerDto model);
}

