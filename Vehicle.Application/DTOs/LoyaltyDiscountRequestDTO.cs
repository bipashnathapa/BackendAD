using System.ComponentModel.DataAnnotations;

namespace Vehicle.Application.DTOs;

public class LoyaltyDiscountRequestDTO
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Purchase amount must be greater than 0.")]
    public decimal PurchaseAmount { get; set; }
}
