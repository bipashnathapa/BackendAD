using System.ComponentModel.DataAnnotations;

namespace Vehicle.Application.DTOs;

public class StaffAppointmentItemDto
{
    public int AppointmentId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public int VehicleId { get; set; }
    public string VehicleNo { get; set; } = string.Empty;
    public string VehicleBrand { get; set; } = string.Empty;
    public string? VehicleModel { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class StaffPartRequestItemDto
{
    public int PartRequestId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public int? VehicleId { get; set; }
    public string? VehicleNo { get; set; }
    public string? VehicleBrand { get; set; }
    public string? VehicleModel { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
}

public class StaffServiceReviewItemDto
{
    public int ServiceReviewId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public int? AppointmentId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime ReviewedAt { get; set; }
}

public class UpdateServiceStatusDto
{
    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = string.Empty;
}

