using Microsoft.AspNetCore.Identity;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;
using Vehicle.Domain.Models;

namespace Vehicle.Infrastructure.Service;

public class CustomerProfileService : ICustomerProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomerProfileService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<CustomerProfileDto?> GetMyProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        return new CustomerProfileDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            Dob = user.DOB
        };
    }

    public async Task<IdentityResult> UpdateMyProfileAsync(string userId, UpdateCustomerProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "NotFound",
                Description = "User not found."
            });
        }

        user.FullName = dto.FullName.Trim();
        user.Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim();

        if (dto.Dob.HasValue)
        {
            // PostgreSQL 'timestamp with time zone' expects UTC
            user.DOB = DateTime.SpecifyKind(dto.Dob.Value, DateTimeKind.Utc);
        }
        else
        {
            user.DOB = null;
        }

        var phone = string.IsNullOrWhiteSpace(dto.PhoneNumber) ? null : dto.PhoneNumber.Trim();
        if (!string.Equals(user.PhoneNumber, phone, StringComparison.Ordinal))
        {
            var setPhone = await _userManager.SetPhoneNumberAsync(user, phone);
            if (!setPhone.Succeeded) return setPhone;
        }

        return await _userManager.UpdateAsync(user);
    }
}
