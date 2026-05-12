namespace Vehicle.Application.DTOs;

public class StaffRegisterCustomerResultDto
{
    public int CustomerId { get; set; }
    public int VehicleId { get; set; }
    public required string UserId { get; set; }
    public required string Message { get; set; }
}

