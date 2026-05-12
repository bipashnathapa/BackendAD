using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;

namespace Vehicle.Infrastructure.Service;

public class LoyaltyService : ILoyaltyService
{
    private const decimal MinimumPurchaseAmount = 5000m;
    private const decimal DiscountRate = 0.10m;

    public LoyaltyDiscountResultDTO CalculateDiscount(decimal purchaseAmount)
    {
        var discountApplied = purchaseAmount > MinimumPurchaseAmount;
        var discountAmount = discountApplied ? Math.Round(purchaseAmount * DiscountRate, 2) : 0m;
        var finalAmount = Math.Round(purchaseAmount - discountAmount, 2);

        return new LoyaltyDiscountResultDTO
        {
            PurchaseAmount = purchaseAmount,
            DiscountPercentage = discountApplied ? DiscountRate * 100m : 0m,
            DiscountAmount = discountAmount,
            FinalAmount = finalAmount,
            DiscountApplied = discountApplied,
            Message = discountApplied
                ? "Loyalty discount applied."
                : "Spend more than 5000 in a single purchase to get a 10% discount."
        };
    }
}
