using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;

namespace VehicleManagement.Controllers;

[Authorize(Roles = "Staff,Admin")]
[Route("api/staff")]
[ApiController]
public class StaffApiController : ControllerBase
{
    private readonly IStaffService _staffService;

    public StaffApiController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [HttpPost("customers")]
    [HttpPost("customers/request-otp")]
    public async Task<IActionResult> RequestCustomerRegistrationOtp(StaffRegisterCustomerDto model)
    {
        IdentityResult result;
        try
        {
            result = await _staffService.RequestCustomerRegistrationOtpAsync(model, HttpContext.RequestAborted);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        if (result.Succeeded)
            return Ok(new { message = "OTP sent to the customer's email. Verify the code to save the customer." });

        return BadRequest(result.Errors);
    }

    [HttpPost("customers/verify-otp")]
    public async Task<IActionResult> VerifyCustomerRegistrationOtp(OtpVerificationDto model)
    {
        var (result, data) = await _staffService.VerifyCustomerRegistrationOtpAsync(model);
        if (result.Succeeded && data != null) return Ok(data);

        return BadRequest(result.Errors);
    }

    [HttpGet("customers")]
    public async Task<IActionResult> SearchCustomers([FromQuery] string search = "", [FromQuery] int take = 20)
    {
        var results = await _staffService.SearchCustomersAsync(search, take);
        return Ok(results);
    }

    [HttpGet("customers/{id:int}")]
    public async Task<IActionResult> GetCustomerHistory(int id)
    {
        var details = await _staffService.GetCustomerHistoryAsync(id);
        return details == null ? NotFound() : Ok(details);
    }
}
