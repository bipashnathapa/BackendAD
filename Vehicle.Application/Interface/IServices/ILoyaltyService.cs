using Vehicle.Application.DTOs;

namespace Vehicle.Application.Interface.IServices;

public interface ILoyaltyService
{
    LoyaltyDiscountResultDTO CalculateDiscount(decimal purchaseAmount);
}
