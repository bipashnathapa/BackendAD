namespace Vehicle.Application.DTOs;

public class CustomerVehicleDto
{
    public int VehicleId { get; set; }
    public required string VehicleNo { get; set; }
    public required string Brand { get; set; }
    public string? Model { get; set; }
    public string? Type { get; set; }
}

