using System;

namespace Vehicle.Application.DTOs;

public class StaffAppointmentDto
{
    public int AppointmentId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public required string ServiceType { get; set; }
    public required string VehicleInfo { get; set; } // Brand + Model
    public required string PlateNumber { get; set; }
    public required string Status { get; set; }
    public decimal? EstimatedCost { get; set; }
}
