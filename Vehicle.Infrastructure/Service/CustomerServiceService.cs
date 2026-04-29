using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IRepositories;
using Vehicle.Application.Interface.IServices;
using Vehicle.Domain.Models;

namespace Vehicle.Infrastructure.Service;

public class CustomerServiceService : ICustomerServiceService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerServiceRepository _customerServiceRepository;

    public CustomerServiceService(
        ICustomerRepository customerRepository,
        ICustomerServiceRepository customerServiceRepository)
    {
        _customerRepository = customerRepository;
        _customerServiceRepository = customerServiceRepository;
    }

    public async Task<AppointmentDTO?> BookAppointmentAsync(string userId, BookAppointmentDTO dto)
    {
        var customer = await _customerRepository.GetByUserIdAsync(userId);
        if (customer == null) return null;

        var ownsVehicle = await _customerServiceRepository.CustomerOwnsVehicleAsync(customer.CustomerID, dto.VehicleId);
        if (!ownsVehicle) return null;

        var appointment = new Appointment
        {
            CustomerID = customer.CustomerID,
            VehicleID = dto.VehicleId,
            AppointmentDate = dto.AppointmentDate,
            ServiceType = dto.ServiceType.Trim(),
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim()
        };

        var savedAppointment = await _customerServiceRepository.AddAppointmentAsync(appointment);
        return new AppointmentDTO
        {
            AppointmentId = savedAppointment.AppointmentID,
            VehicleId = savedAppointment.VehicleID,
            VehicleNo = string.Empty,
            ServiceType = savedAppointment.ServiceType,
            AppointmentDate = savedAppointment.AppointmentDate,
            Notes = savedAppointment.Notes,
            Status = savedAppointment.Status,
            CreatedAt = savedAppointment.CreatedAt
        };
    }

    public async Task<IReadOnlyList<AppointmentDTO>> GetMyAppointmentsAsync(string userId)
    {
        var customer = await _customerRepository.GetByUserIdAsync(userId);
        if (customer == null) return Array.Empty<AppointmentDTO>();

        var appointments = await _customerServiceRepository.GetAppointmentsByCustomerIdAsync(customer.CustomerID);
        return appointments.Select(a => new AppointmentDTO
        {
            AppointmentId = a.AppointmentID,
            VehicleId = a.VehicleID,
            VehicleNo = a.Vehicle.VehicleNo,
            ServiceType = a.ServiceType,
            AppointmentDate = a.AppointmentDate,
            Notes = a.Notes,
            Status = a.Status,
            CreatedAt = a.CreatedAt
        }).ToList();
    }

    public async Task<PartRequestDTO?> RequestUnavailablePartAsync(string userId, RequestPartDTO dto)
    {
        var customer = await _customerRepository.GetByUserIdAsync(userId);
        if (customer == null) return null;

        if (dto.VehicleId.HasValue)
        {
            var ownsVehicle = await _customerServiceRepository.CustomerOwnsVehicleAsync(customer.CustomerID, dto.VehicleId.Value);
            if (!ownsVehicle) return null;
        }

        var partRequest = new PartRequest
        {
            CustomerID = customer.CustomerID,
            VehicleID = dto.VehicleId,
            PartName = dto.PartName.Trim(),
            VehicleModel = string.IsNullOrWhiteSpace(dto.VehicleModel) ? null : dto.VehicleModel.Trim(),
            Quantity = dto.Quantity,
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim()
        };

        var savedRequest = await _customerServiceRepository.AddPartRequestAsync(partRequest);
        return new PartRequestDTO
        {
            PartRequestId = savedRequest.PartRequestID,
            VehicleId = savedRequest.VehicleID,
            VehicleNo = null,
            PartName = savedRequest.PartName,
            VehicleModel = savedRequest.VehicleModel,
            Quantity = savedRequest.Quantity,
            Notes = savedRequest.Notes,
            Status = savedRequest.Status,
            RequestedAt = savedRequest.RequestedAt
        };
    }

    public async Task<IReadOnlyList<PartRequestDTO>> GetMyPartRequestsAsync(string userId)
    {
        var customer = await _customerRepository.GetByUserIdAsync(userId);
        if (customer == null) return Array.Empty<PartRequestDTO>();

        var partRequests = await _customerServiceRepository.GetPartRequestsByCustomerIdAsync(customer.CustomerID);
        return partRequests.Select(p => new PartRequestDTO
        {
            PartRequestId = p.PartRequestID,
            VehicleId = p.VehicleID,
            VehicleNo = p.Vehicle?.VehicleNo,
            PartName = p.PartName,
            VehicleModel = p.VehicleModel,
            Quantity = p.Quantity,
            Notes = p.Notes,
            Status = p.Status,
            RequestedAt = p.RequestedAt
        }).ToList();
    }

    public async Task<ServiceReviewDTO?> AddServiceReviewAsync(string userId, CreateServiceReviewDTO dto)
    {
        var customer = await _customerRepository.GetByUserIdAsync(userId);
        if (customer == null) return null;

        if (dto.AppointmentId.HasValue)
        {
            var ownsAppointment = await _customerServiceRepository.CustomerOwnsAppointmentAsync(customer.CustomerID, dto.AppointmentId.Value);
            if (!ownsAppointment) return null;
        }

        var review = new ServiceReview
        {
            CustomerID = customer.CustomerID,
            AppointmentID = dto.AppointmentId,
            Rating = dto.Rating,
            Comment = dto.Comment.Trim()
        };

        var savedReview = await _customerServiceRepository.AddServiceReviewAsync(review);
        return new ServiceReviewDTO
        {
            ServiceReviewId = savedReview.ServiceReviewID,
            AppointmentId = savedReview.AppointmentID,
            Rating = savedReview.Rating,
            Comment = savedReview.Comment,
            ReviewedAt = savedReview.ReviewedAt
        };
    }

    public async Task<IReadOnlyList<ServiceReviewDTO>> GetMyServiceReviewsAsync(string userId)
    {
        var customer = await _customerRepository.GetByUserIdAsync(userId);
        if (customer == null) return Array.Empty<ServiceReviewDTO>();

        var reviews = await _customerServiceRepository.GetServiceReviewsByCustomerIdAsync(customer.CustomerID);
        return reviews.Select(r => new ServiceReviewDTO
        {
            ServiceReviewId = r.ServiceReviewID,
            AppointmentId = r.AppointmentID,
            Rating = r.Rating,
            Comment = r.Comment,
            ReviewedAt = r.ReviewedAt
        }).ToList();
    }
}
