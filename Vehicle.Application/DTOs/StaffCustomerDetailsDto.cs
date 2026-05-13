using System.Collections.Generic;

namespace Vehicle.Application.DTOs;

public class StaffCustomerDetailsDto
{
    public int CustomerId { get; set; }
    public required string UserId { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    public List<StaffCustomerVehicleDto> Vehicles { get; set; } = new();
    public List<StaffAppointmentDto> Appointments { get; set; } = new();
}
