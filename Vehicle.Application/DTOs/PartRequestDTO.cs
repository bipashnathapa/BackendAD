namespace Vehicle.Application.DTOs;

public class PartRequestDTO
{
    public int PartRequestId { get; set; }
    public int? VehicleId { get; set; }
    public string? VehicleNo { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string? VehicleModel { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
}
