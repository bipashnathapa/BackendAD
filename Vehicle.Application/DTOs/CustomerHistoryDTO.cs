namespace Vehicle.Application.DTOs;

public class CustomerHistoryDTO
{
    public int ReferenceId { get; set; }
    public string HistoryType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? VehicleNo { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
