using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;
using Vehicle.Domain.Models;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Service;

public class AdminStaffService : IAdminStaffService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;

    public AdminStaffService(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public async Task<(bool Ok, string Message)> CreateStaffAsync(AdminCreateStaffDto dto)
    {
        var email = dto.Email.Trim();
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing != null) return (false, "A user with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = dto.FullName.Trim(),
            UserRole = "Staff"
        };

        await using var tx = await _db.Database.BeginTransactionAsync();
        var created = await _userManager.CreateAsync(user, dto.Password);
        if (!created.Succeeded)
        {
            var msg = string.Join(", ", created.Errors.Select(e => e.Description));
            await tx.RollbackAsync();
            return (false, msg);
        }

        // Ensure Identity role matches the UserRole field used for JWT claims/authorization.
        await _userManager.AddToRoleAsync(user, "Staff");

        _db.Staffs.Add(new Staff { UserID = user.Id });
        await _db.SaveChangesAsync();

        await tx.CommitAsync();
        return (true, "Staff account created.");
    }

    public async Task<IReadOnlyList<AdminStaffListItemDto>> GetStaffAsync(int take = 100)
    {
        take = Math.Clamp(take, 1, 200);
        return await _userManager.Users
            .AsNoTracking()
            .Where(u => u.UserRole == "Staff")
            .OrderBy(u => u.FullName)
            .Take(take)
            .Select(u => new AdminStaffListItemDto
            {
                UserId = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? ""
            })
            .ToListAsync();
    }
}

