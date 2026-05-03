namespace Vehicle.Application.DTOs;

public class LoyaltyDiscountResultDTO
{
    public decimal PurchaseAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public bool DiscountApplied { get; set; }
    public string Message { get; set; } = string.Empty;
}
