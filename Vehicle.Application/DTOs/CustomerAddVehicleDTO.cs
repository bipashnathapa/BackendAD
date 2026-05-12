using System.ComponentModel.DataAnnotations;

namespace Vehicle.Application.DTOs;

public class CustomerAddVehicleDTO
{
    [Required]
    public required string VehicleNo { get; set; }

    [Required]
    public required string Brand { get; set; }

    public string? Model { get; set; }
    public string? Type { get; set; }
}

