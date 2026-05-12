using Microsoft.AspNetCore.Identity;
using Vehicle.Application.DTOs;

namespace Vehicle.Application.Interface.IServices;

public interface IAuthService
{
    Task<IdentityResult> RegisterAsync(RegisterDTO model);
    Task<IdentityResult> RequestRegistrationOtpAsync(RegisterDTO model, CancellationToken cancellationToken = default);
    Task<IdentityResult> VerifyRegistrationOtpAsync(OtpVerificationDto model);
    Task<string?> LoginAsync(LoginDTO model);
}
