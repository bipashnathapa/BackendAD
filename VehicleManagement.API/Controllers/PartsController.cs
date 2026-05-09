using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;

namespace VehicleManagement.Controllers;

[ApiController]
[Route("api/parts")]
[Authorize]
public class PartsController : ControllerBase
{
    private readonly IPartService _svc;
    public PartsController(IPartService svc) => _svc = svc;

    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _svc.GetByIdAsync(id);
        return p == null ? NotFound() : Ok(p);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePartDTO dto)
    {
        var p = await _svc.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = p.PartID }, p);
    }

    [HttpGet("low-stock")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetLowStock() => Ok(await _svc.GetLowStockAsync());
}
