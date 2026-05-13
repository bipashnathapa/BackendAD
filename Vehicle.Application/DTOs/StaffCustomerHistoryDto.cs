using System.Collections.Generic;

namespace Vehicle.Application.DTOs;

public class StaffCustomerHistoryDto
{
    public int CustomerId { get; set; }
    public required string UserId { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    public List<StaffCustomerHistoryVehicleDto> Vehicles { get; set; } = new();
    public List<StaffCustomerHistoryInvoiceDto> Invoices { get; set; } = new();
}

public class StaffCustomerHistoryVehicleDto
{
    public int VehicleId { get; set; }
    public required string VehicleNo { get; set; }
    public required string Brand { get; set; }
    public string? Model { get; set; }
    public string? Type { get; set; }
}

public class StaffCustomerHistoryInvoiceDto
{
    public int InvoiceId { get; set; }
    public required string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountDue { get; set; }
    public required string PaymentStatus { get; set; }
    public string? PaymentMethod { get; set; }
}
