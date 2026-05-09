using Vehicle.Application.DTOs;

namespace Vehicle.Application.Interface.IRepositories;

public interface IReportRepository
{
    Task<IReadOnlyList<RegularCustomerDTO>> GetRegularCustomersAsync(int minVisits, int days);
    Task<IReadOnlyList<HighSpenderDTO>> GetHighSpendersAsync(int top);
    Task<IReadOnlyList<PendingCreditDTO>> GetPendingCreditsAsync(int olderThanDays);
}
