using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IRepositories;
using Vehicle.Application.Interface.IServices;

namespace Vehicle.Infrastructure.Service;

public class ReportService : IReportService
{
    private readonly IReportRepository _repo;
    public ReportService(IReportRepository repo) => _repo = repo;

    public Task<IReadOnlyList<RegularCustomerDTO>> GetRegularsAsync(int minVisits = 3, int days = 180) =>
        _repo.GetRegularCustomersAsync(minVisits, days);

    public Task<IReadOnlyList<HighSpenderDTO>> GetHighSpendersAsync(int top = 10) =>
        _repo.GetHighSpendersAsync(top);

    public Task<IReadOnlyList<PendingCreditDTO>> GetPendingCreditsAsync(int olderThanDays = 30) =>
        _repo.GetPendingCreditsAsync(olderThanDays);
}
