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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ICustomerRepository _customerRepository;

    public StaffService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        ICustomerRepository customerRepository)
    {
        _userManager = userManager;
        _context = context;
        _customerRepository = customerRepository;
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
            UserRole = "Customer"
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
            Model = model.Model,
            Type = model.Type,
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
}
