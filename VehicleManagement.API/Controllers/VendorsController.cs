using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;

namespace VehicleManagement.Controllers;

[ApiController]
[Route("api/admin/vendors")]
[Authorize(Roles = "Admin")]
public class VendorsController : ControllerBase
{
    private readonly IVendorService _svc;
    public VendorsController(IVendorService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var v = await _svc.GetByIdAsync(id);
        return v == null ? NotFound() : Ok(v);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVendorDTO dto)
    {
        var created = await _svc.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.VendorID }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateVendorDTO dto)
    {
        var ok = await _svc.UpdateAsync(id, dto);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _svc.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
