using System.Collections.Concurrent;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IRepositories;
using Vehicle.Application.Interface.IServices;
using Vehicle.Domain.Models;
using Vehicle.Infrastructure.Data;
using System.Linq;

namespace Vehicle.Infrastructure.Service;

public class StaffService : IStaffService
{
    private const string StaffCustomerOtpPurpose = "staff-customer-registration";
    private static readonly TimeSpan PendingCustomerExpiry = TimeSpan.FromMinutes(10);
    private static readonly ConcurrentDictionary<string, PendingStaffCustomerRegistration> PendingCustomerRegistrations = new();

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEmailOtpService _emailOtpService;

    public StaffService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        ICustomerRepository customerRepository,
        IEmailOtpService emailOtpService)
    {
        _userManager = userManager;
        _context = context;
        _customerRepository = customerRepository;
        _emailOtpService = emailOtpService;
    }

    public async Task<(IdentityResult Result, StaffRegisterCustomerResultDto? Data)> RegisterCustomerWithVehicleAsync(StaffRegisterCustomerDto model)
    {
        // Avoid duplicate Identity users
        var existing = await _userManager.FindByEmailAsync(model.Email);
        if (existing != null)
        {
            return (IdentityResult.Failed(new IdentityError
            {
                Code = "DuplicateEmail",
                Description = "A user with this email already exists."
            }), null);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            Address = model.Address,
            UserRole = "Customer",
            EmailConfirmed = true
        };

        // Transaction keeps Identity + app tables consistent
        await using var tx = await _context.Database.BeginTransactionAsync();
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            await tx.RollbackAsync();
            return (result, null);
        }

        var customer = new Customer { UserID = user.Id };
        await _customerRepository.AddAsync(customer);

        var vehicle = new VehicleInfo
        {
            VehicleNo = model.VehicleNo,
            Brand = model.Brand,
            Model = model.Model ?? "",
            Type = model.Type ?? "",
            CustomerID = customer.CustomerID
        };

        await _context.Vehicles.AddAsync(vehicle);
        await _context.SaveChangesAsync();

        await tx.CommitAsync();

        return (IdentityResult.Success, new StaffRegisterCustomerResultDto
        {
            CustomerId = customer.CustomerID,
            VehicleId = vehicle.VehicleID,
            UserId = user.Id,
            Message = "Customer registered successfully."
        });
    }

    public async Task<IdentityResult> RequestCustomerRegistrationOtpAsync(StaffRegisterCustomerDto model, CancellationToken cancellationToken = default)
    {
        var existing = await _userManager.FindByEmailAsync(model.Email);
        if (existing != null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "DuplicateEmail",
                Description = "A user with this email already exists."
            });
        }

        var key = NormalizeEmail(model.Email);
        PendingCustomerRegistrations[key] = new PendingStaffCustomerRegistration
        {
            Model = model,
            ExpiresAt = DateTimeOffset.UtcNow.Add(PendingCustomerExpiry)
        };

        try
        {
            await _emailOtpService.SendOtpAsync(model.Email, StaffCustomerOtpPurpose, model.FullName, cancellationToken);
        }
        catch
        {
            PendingCustomerRegistrations.TryRemove(key, out _);
            throw;
        }

        return IdentityResult.Success;
    }

    public async Task<(IdentityResult Result, StaffRegisterCustomerResultDto? Data)> VerifyCustomerRegistrationOtpAsync(OtpVerificationDto model)
    {
        var key = NormalizeEmail(model.Email);
        if (!PendingCustomerRegistrations.TryGetValue(key, out var pending) || DateTimeOffset.UtcNow > pending.ExpiresAt)
        {
            PendingCustomerRegistrations.TryRemove(key, out _);
            return (IdentityResult.Failed(new IdentityError
            {
                Code = "OtpExpired",
                Description = "OTP expired. Please request a new code."
            }), null);
        }

        if (!_emailOtpService.VerifyOtp(model.Email, StaffCustomerOtpPurpose, model.OtpCode))
        {
            return (IdentityResult.Failed(new IdentityError
            {
                Code = "InvalidOtp",
                Description = "Invalid OTP code."
            }), null);
        }

        var result = await RegisterCustomerWithVehicleAsync(pending.Model);
        if (result.Result.Succeeded)
        {
            PendingCustomerRegistrations.TryRemove(key, out _);
            _emailOtpService.ClearOtp(model.Email, StaffCustomerOtpPurpose);
        }

        return result;
    }

    public async Task<IReadOnlyList<StaffCustomerSearchResultDto>> SearchCustomersAsync(string search, int take = 20)
    {
        var customers = await _customerRepository.SearchAsync(search, take);

        return customers.Select(c => new StaffCustomerSearchResultDto
        {
            CustomerId = c.CustomerID,
            UserId = c.UserID,
            FullName = c.User?.FullName ?? "",
            Email = c.User?.Email ?? "",
            PhoneNumber = c.User?.PhoneNumber,
            Vehicles = (c.Vehicles ?? new List<VehicleInfo>())
                .Select(v => new StaffCustomerVehicleDto
                {
                    VehicleId = v.VehicleID,
                    VehicleNo = v.VehicleNo,
                    Brand = v.Brand,
                    Model = v.Model,
                    Type = v.Type,
                }).ToList()
        }).ToList();
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToUpperInvariant();
    }

    private sealed class PendingStaffCustomerRegistration
    {
        public StaffRegisterCustomerDto Model { get; set; } = null!;
        public DateTimeOffset ExpiresAt { get; set; }
    public async Task<StaffCustomerDetailsDto?> GetCustomerDetailsAsync(int customerId)
    {
        var customer = await _context.Customers
            .Include(c => c.User)
            .Include(c => c.Vehicles)
            .Include(c => c.Appointments)
                .ThenInclude(a => a.Vehicle)
            .FirstOrDefaultAsync(c => c.CustomerID == customerId);

        if (customer == null) return null;

        return new StaffCustomerDetailsDto
        {
            CustomerId = customer.CustomerID,
            UserId = customer.UserID,
            FullName = customer.User?.FullName ?? "Unknown",
            Email = customer.User?.Email ?? "N/A",
            PhoneNumber = customer.User?.PhoneNumber,
            Address = customer.User?.Address,
            Vehicles = customer.Vehicles.Select(v => new StaffCustomerVehicleDto
            {
                VehicleId = v.VehicleID,
                VehicleNo = v.VehicleNo,
                Brand = v.Brand,
                Model = v.Model,
                Type = v.Type
            }).ToList(),
            Appointments = customer.Appointments.OrderByDescending(a => a.AppointmentDate).Select(a => new StaffAppointmentDto
            {
                AppointmentId = a.AppointmentID,
                AppointmentDate = a.AppointmentDate,
                ServiceType = a.ServiceType,
                VehicleInfo = $"{a.Vehicle?.Brand} {a.Vehicle?.Model}".Trim(),
                PlateNumber = a.Vehicle?.VehicleNo ?? "N/A",
                Status = a.Status
            }).ToList()
        };
    }
}
