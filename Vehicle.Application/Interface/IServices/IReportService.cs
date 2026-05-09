using Vehicle.Application.DTOs;

namespace Vehicle.Application.Interface.IServices;

public interface IReportService
{
    Task<IReadOnlyList<RegularCustomerDTO>> GetRegularsAsync(int minVisits = 3, int days = 180);
    Task<IReadOnlyList<HighSpenderDTO>> GetHighSpendersAsync(int top = 10);
    Task<IReadOnlyList<PendingCreditDTO>> GetPendingCreditsAsync(int olderThanDays = 30);
}
