using System.ComponentModel.DataAnnotations;

namespace Vehicle.Application.DTOs;

public class CustomerAddVehicleDTO
{
    [Required]
    public string VehicleNo { get; set; }

    [Required]
    public string Brand { get; set; }

    public string? Model { get; set; }
    public string? Type { get; set; }
}

