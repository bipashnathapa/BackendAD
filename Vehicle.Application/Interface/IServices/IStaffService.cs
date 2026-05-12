using Microsoft.AspNetCore.Identity;
using Vehicle.Application.DTOs;

namespace Vehicle.Application.Interface.IServices;

public interface IStaffService
{
    Task<(IdentityResult Result, StaffRegisterCustomerResultDto? Data)> RegisterCustomerWithVehicleAsync(StaffRegisterCustomerDto model);
    Task<IdentityResult> RequestCustomerRegistrationOtpAsync(StaffRegisterCustomerDto model, CancellationToken cancellationToken = default);
    Task<(IdentityResult Result, StaffRegisterCustomerResultDto? Data)> VerifyCustomerRegistrationOtpAsync(OtpVerificationDto model);
    Task<IReadOnlyList<StaffCustomerSearchResultDto>> SearchCustomersAsync(string search, int take = 20);
}
