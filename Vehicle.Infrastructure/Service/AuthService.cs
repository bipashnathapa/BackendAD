using System.Collections.Concurrent;
using Microsoft.AspNetCore.Identity;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;
using Vehicle.Domain.Models;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Service;

public class AuthService : IAuthService
{
    private const string RegistrationOtpPurpose = "account-registration";
    private static readonly TimeSpan PendingRegistrationExpiry = TimeSpan.FromMinutes(10);
    private static readonly ConcurrentDictionary<string, PendingRegistration> PendingRegistrations = new();

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IEmailOtpService _emailOtpService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        IJwtTokenService jwtTokenService,
        IEmailOtpService emailOtpService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _jwtTokenService = jwtTokenService;
        _emailOtpService = emailOtpService;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterDTO model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            Address = model.Address,
            UserRole = model.UserRole
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            if (model.UserRole.Equals("Customer", StringComparison.OrdinalIgnoreCase))
            {
                _context.Customers.Add(new Customer { UserID = user.Id });
            }
            else if (model.UserRole.Equals("Staff", StringComparison.OrdinalIgnoreCase))
            {
                _context.Staffs.Add(new Staff { UserID = user.Id });
            }

            await _context.SaveChangesAsync();

            if (!await _roleManager.RoleExistsAsync(model.UserRole))
                await _roleManager.CreateAsync(new IdentityRole(model.UserRole));
            await _userManager.AddToRoleAsync(user, model.UserRole);
        }

        return result;
    }

    public async Task<IdentityResult> RequestRegistrationOtpAsync(RegisterDTO model, CancellationToken cancellationToken = default)
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

        if (!model.UserRole.Equals("Customer", StringComparison.OrdinalIgnoreCase) &&
            !model.UserRole.Equals("Staff", StringComparison.OrdinalIgnoreCase))
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "InvalidRole",
                Description = "User role must be Customer or Staff."
            });
        }

        var key = NormalizeEmail(model.Email);
        PendingRegistrations[key] = new PendingRegistration
        {
            Model = model,
            ExpiresAt = DateTimeOffset.UtcNow.Add(PendingRegistrationExpiry)
        };

        try
        {
            await _emailOtpService.SendOtpAsync(model.Email, RegistrationOtpPurpose, model.FullName, cancellationToken);
        }
        catch
        {
            PendingRegistrations.TryRemove(key, out _);
            throw;
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> VerifyRegistrationOtpAsync(OtpVerificationDto model)
    {
        var key = NormalizeEmail(model.Email);
        if (!PendingRegistrations.TryGetValue(key, out var pending) || DateTimeOffset.UtcNow > pending.ExpiresAt)
        {
            PendingRegistrations.TryRemove(key, out _);
            return IdentityResult.Failed(new IdentityError
            {
                Code = "OtpExpired",
                Description = "OTP expired. Please request a new code."
            });
        }

        if (!_emailOtpService.VerifyOtp(model.Email, RegistrationOtpPurpose, model.OtpCode))
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "InvalidOtp",
                Description = "Invalid OTP code."
            });
        }

        var result = await RegisterAsync(pending.Model);
        if (result.Succeeded)
        {
            PendingRegistrations.TryRemove(key, out _);
            _emailOtpService.ClearOtp(model.Email, RegistrationOtpPurpose);
        }

        return result;
    }

    public async Task<string?> LoginAsync(LoginDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return _jwtTokenService.GenerateToken(user);
        }

        return null;
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToUpperInvariant();
    }

    private sealed class PendingRegistration
    {
        public RegisterDTO Model { get; set; } = null!;
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
