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
        var result = await _authService.RegisterAsync(model);
        
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