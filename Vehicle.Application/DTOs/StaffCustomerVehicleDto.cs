namespace Vehicle.Application.DTOs;

public class StaffCustomerVehicleDto
{
    public int VehicleId { get; set; }
    public required string VehicleNo { get; set; }
    public required string Brand { get; set; }
    public string? Model { get; set; }
    public string? Type { get; set; }
}

