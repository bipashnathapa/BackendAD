using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;
using Vehicle.Domain.Models; // For ApplicationUser, Customer, Staff
using Vehicle.Infrastructure.Data; // For ApplicationDbContext

namespace VehicleManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO model)
    {
        return await RequestRegistrationOtp(model);
    }

    [HttpPost("register/request-otp")]
    public async Task<IActionResult> RequestRegistrationOtp(RegisterDTO model)
    {
        IdentityResult result;
        try
        {
            result = await _authService.RequestRegistrationOtpAsync(model, HttpContext.RequestAborted);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        
        if (result.Succeeded) 
            return Ok(new { message = "OTP sent to the email address. Verify the code to complete registration." });

        return BadRequest(result.Errors);
    }

    [HttpPost("register/verify-otp")]
    public async Task<IActionResult> VerifyRegistrationOtp(OtpVerificationDto model)
    {
        var result = await _authService.VerifyRegistrationOtpAsync(model);

        if (result.Succeeded)
            return Ok(new { message = "Registration successful!" });

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO model)
    {
        var token = await _authService.LoginAsync(model);
        
        if (token == null) 
            return Unauthorized(new { message = "Invalid login attempt." });

        return Ok(new { token = token, message = "Login successful!" });
    }
}
