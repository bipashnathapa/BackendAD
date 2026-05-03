namespace Vehicle.Application.DTOs;

public class ServiceReviewDTO
{
    public int ServiceReviewId { get; set; }
    public int? AppointmentId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime ReviewedAt { get; set; }
}
