using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;

namespace VehicleManagement.Controllers;

[ApiController]
[Route("api/customer/services")]
[Authorize(Roles = "Customer")]
public class CustomerServicesController : ControllerBase
{
    private readonly ICustomerServiceService _customerServiceService;

    public CustomerServicesController(ICustomerServiceService customerServiceService)
    {
        _customerServiceService = customerServiceService;
    }

    [HttpPost("appointments")]
    public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentDTO dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var appointment = await _customerServiceService.BookAppointmentAsync(userId, dto);
        return appointment == null
            ? BadRequest(new { message = "Unable to book appointment. Check customer and vehicle details." })
            : Ok(appointment);
    }

    [HttpGet("appointments")]
    public async Task<IActionResult> GetMyAppointments()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var appointments = await _customerServiceService.GetMyAppointmentsAsync(userId);
        return Ok(appointments);
    }

    [HttpPost("parts")]
    public async Task<IActionResult> RequestUnavailablePart([FromBody] RequestPartDTO dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var partRequest = await _customerServiceService.RequestUnavailablePartAsync(userId, dto);
        return partRequest == null
            ? BadRequest(new { message = "Unable to request part. Check customer and vehicle details." })
            : Ok(partRequest);
    }

    [HttpGet("parts")]
    public async Task<IActionResult> GetMyPartRequests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var partRequests = await _customerServiceService.GetMyPartRequestsAsync(userId);
        return Ok(partRequests);
    }

    [HttpPost("reviews")]
    public async Task<IActionResult> AddServiceReview([FromBody] CreateServiceReviewDTO dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var review = await _customerServiceService.AddServiceReviewAsync(userId, dto);
        return review == null
            ? BadRequest(new { message = "Unable to add review. Check customer and appointment details." })
            : Ok(review);
    }

    [HttpGet("reviews")]
    public async Task<IActionResult> GetMyServiceReviews()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var reviews = await _customerServiceService.GetMyServiceReviewsAsync(userId);
        return Ok(reviews);
    }
}
