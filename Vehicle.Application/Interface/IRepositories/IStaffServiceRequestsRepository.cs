using Vehicle.Domain.Models;

namespace Vehicle.Application.Interface.IRepositories;

public interface IStaffServiceRequestsRepository
{
    Task<IReadOnlyList<Appointment>> GetAppointmentsAsync(string? status, int take);
    Task<Appointment?> GetAppointmentByIdAsync(int id);

    Task<IReadOnlyList<PartRequest>> GetPartRequestsAsync(string? status, int take);
    Task<PartRequest?> GetPartRequestByIdAsync(int id);

    Task<IReadOnlyList<ServiceReview>> GetServiceReviewsAsync(int take);

    Task SaveChangesAsync();
}

