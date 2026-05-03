using Vehicle.Application.DTOs;

namespace Vehicle.Application.Interface.IServices;

public interface ICustomerServiceService
{
    Task<AppointmentDTO?> BookAppointmentAsync(string userId, BookAppointmentDTO dto);
    Task<IReadOnlyList<AppointmentDTO>> GetMyAppointmentsAsync(string userId);
    Task<PartRequestDTO?> RequestUnavailablePartAsync(string userId, RequestPartDTO dto);
    Task<IReadOnlyList<PartRequestDTO>> GetMyPartRequestsAsync(string userId);
    Task<ServiceReviewDTO?> AddServiceReviewAsync(string userId, CreateServiceReviewDTO dto);
    Task<IReadOnlyList<ServiceReviewDTO>> GetMyServiceReviewsAsync(string userId);
}
