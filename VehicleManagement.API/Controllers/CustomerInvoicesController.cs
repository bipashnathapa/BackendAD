using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vehicle.Infrastructure.Data;

namespace VehicleManagement.Controllers;

[ApiController]
[Route("api/customer/invoices")]
[Authorize(Roles = "Customer")]
public class CustomerInvoicesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public CustomerInvoicesController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.UserID == userId);
        if (customer == null) return Ok(Array.Empty<object>());

        var invoices = await _db.SaleInvoices
            .Where(i => i.CustomerID == customer.CustomerID)
            .OrderByDescending(i => i.InvoiceDate)
            .Select(i => new
            {
                i.InvoiceID, i.InvoiceNumber, i.InvoiceDate,
                i.TotalAmount, i.AmountPaid, i.AmountDue,
                i.PaymentStatus, i.PaymentMethod
            })
            .ToListAsync();
        return Ok(invoices);
    }
}
