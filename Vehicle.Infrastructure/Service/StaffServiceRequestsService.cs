using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IRepositories;
using Vehicle.Application.Interface.IServices;

namespace Vehicle.Infrastructure.Service;

public class StaffServiceRequestsService : IStaffServiceRequestsService
{
    private static readonly HashSet<string> AppointmentStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pending",
        "Approved",
        "Completed",
        "Cancelled"
    };

    private static readonly HashSet<string> PartRequestStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Requested",
        "Ordered",
        "Received",
        "Cancelled"
    };

    private readonly IStaffServiceRequestsRepository _repo;

    public StaffServiceRequestsService(IStaffServiceRequestsRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<StaffAppointmentItemDto>> GetAppointmentsAsync(string? status, int take)
    {
        var items = await _repo.GetAppointmentsAsync(status, take);
        return items.Select(a => new StaffAppointmentItemDto
        {
            AppointmentId = a.AppointmentID,
            CustomerId = a.CustomerID,
            CustomerName = a.Customer?.User?.FullName ?? "",
            CustomerEmail = a.Customer?.User?.Email ?? "",
            VehicleId = a.VehicleID,
            VehicleNo = a.Vehicle?.VehicleNo ?? "",
            VehicleBrand = a.Vehicle?.Brand ?? "",
            VehicleModel = a.Vehicle?.Model,
            AppointmentDate = a.AppointmentDate,
            ServiceType = a.ServiceType,
            Notes = a.Notes,
            Status = a.Status,
            CreatedAt = a.CreatedAt
        }).ToList();
    }

    public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status)
    {
        if (string.IsNullOrWhiteSpace(status)) return false;
        status = status.Trim();
        if (!AppointmentStatuses.Contains(status)) return false;

        var a = await _repo.GetAppointmentByIdAsync(appointmentId);
        if (a == null) return false;

        a.Status = status;
        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<StaffPartRequestItemDto>> GetPartRequestsAsync(string? status, int take)
    {
        var items = await _repo.GetPartRequestsAsync(status, take);
        return items.Select(p => new StaffPartRequestItemDto
        {
            PartRequestId = p.PartRequestID,
            CustomerId = p.CustomerID,
            CustomerName = p.Customer?.User?.FullName ?? "",
            CustomerEmail = p.Customer?.User?.Email ?? "",
            VehicleId = p.VehicleID,
            VehicleNo = p.Vehicle?.VehicleNo,
            VehicleBrand = p.Vehicle?.Brand,
            VehicleModel = p.VehicleModel ?? p.Vehicle?.Model,
            PartName = p.PartName,
            Quantity = p.Quantity,
            Notes = p.Notes,
            Status = p.Status,
            RequestedAt = p.RequestedAt
        }).ToList();
    }

    public async Task<bool> UpdatePartRequestStatusAsync(int partRequestId, string status)
    {
        if (string.IsNullOrWhiteSpace(status)) return false;
        status = status.Trim();
        if (!PartRequestStatuses.Contains(status)) return false;

        var p = await _repo.GetPartRequestByIdAsync(partRequestId);
        if (p == null) return false;

        p.Status = status;
        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<StaffServiceReviewItemDto>> GetServiceReviewsAsync(int take)
    {
        var items = await _repo.GetServiceReviewsAsync(take);
        return items.Select(r => new StaffServiceReviewItemDto
        {
            ServiceReviewId = r.ServiceReviewID,
            CustomerId = r.CustomerID,
            CustomerName = r.Customer?.User?.FullName ?? "",
            CustomerEmail = r.Customer?.User?.Email ?? "",
            AppointmentId = r.AppointmentID,
            Rating = r.Rating,
            Comment = r.Comment,
            ReviewedAt = r.ReviewedAt
        }).ToList();
    }
}

