namespace Vehicle.Application.DTOs;

public class StaffCustomerSearchResultDto
{
    public int CustomerId { get; set; }
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public List<StaffCustomerVehicleDto> Vehicles { get; set; } = new();
}

