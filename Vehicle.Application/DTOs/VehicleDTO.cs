namespace Vehicle.Application.DTOs;

public class VehicleDTO
{
    public required string VehicleNo { get; set; }
    public required string Model { get; set; }
    public required string Brand { get; set; }
    public int CustomerId { get; set; }
    public required string Type { get; set; }
}
