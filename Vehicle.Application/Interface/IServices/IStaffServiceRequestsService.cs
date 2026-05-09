using Vehicle.Application.DTOs;

namespace Vehicle.Application.Interface.IServices;

public interface IStaffServiceRequestsService
{
    Task<IReadOnlyList<StaffAppointmentItemDto>> GetAppointmentsAsync(string? status, int take);
    Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status);

    Task<IReadOnlyList<StaffPartRequestItemDto>> GetPartRequestsAsync(string? status, int take);
    Task<bool> UpdatePartRequestStatusAsync(int partRequestId, string status);

    Task<IReadOnlyList<StaffServiceReviewItemDto>> GetServiceReviewsAsync(int take);
}

