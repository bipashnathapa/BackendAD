namespace Vehicle.Application.DTOs;

public class AppointmentDTO
{
    public int AppointmentId { get; set; }
    public int VehicleId { get; set; }
    public string VehicleNo { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
