using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;

namespace VehicleManagement.Controllers;

[ApiController]
[Route("api/customer/loyalty")]
[Authorize(Roles = "Customer")]
public class LoyaltyController : ControllerBase
{
    private readonly ILoyaltyService _loyaltyService;

    public LoyaltyController(ILoyaltyService loyaltyService)
    {
        _loyaltyService = loyaltyService;
    }

    [HttpPost("calculate")]
    public IActionResult CalculateDiscount([FromBody] LoyaltyDiscountRequestDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = _loyaltyService.CalculateDiscount(dto.PurchaseAmount);
        return Ok(result);
    }
}
