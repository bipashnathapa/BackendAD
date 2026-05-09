using Microsoft.AspNetCore.Identity;
using Vehicle.Application.DTOs;

namespace Vehicle.Application.Interface.IServices;

public interface ICustomerProfileService
{
    Task<CustomerProfileDto?> GetMyProfileAsync(string userId);
    Task<IdentityResult> UpdateMyProfileAsync(string userId, UpdateCustomerProfileDto dto);
}

