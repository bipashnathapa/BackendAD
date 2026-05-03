using Vehicle.Domain.Models;

namespace Vehicle.Application.Interface.IRepositories;

public interface ICustomerServiceRepository
{
    Task<Appointment> AddAppointmentAsync(Appointment appointment);
    Task<IReadOnlyList<Appointment>> GetAppointmentsByCustomerIdAsync(int customerId);
    Task<PartRequest> AddPartRequestAsync(PartRequest partRequest);
    Task<IReadOnlyList<PartRequest>> GetPartRequestsByCustomerIdAsync(int customerId);
    Task<ServiceReview> AddServiceReviewAsync(ServiceReview review);
    Task<IReadOnlyList<ServiceReview>> GetServiceReviewsByCustomerIdAsync(int customerId);
    Task<bool> CustomerOwnsVehicleAsync(int customerId, int vehicleId);
    Task<bool> CustomerOwnsAppointmentAsync(int customerId, int appointmentId);
}
