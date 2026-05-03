using Microsoft.AspNetCore.Identity;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interface.IServices;
using Vehicle.Domain.Models;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Service;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterDTO model)
    {
        // 1. Create the Identity User
        var user = new ApplicationUser 
        { 
            UserName = model.Email, 
            Email = model.Email, 
            FullName = model.FullName,
            Address = model.Address,
            UserRole = model.UserRole 
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        // 2. If Identity creation succeeded, link to Customer/Staff tables
        if (result.Succeeded)
        {
            try
            {
                if (model.UserRole.Equals("Customer", StringComparison.OrdinalIgnoreCase))
                {
                    _context.Customers.Add(new Customer { UserID = user.Id, User = user });
                }
                else if (model.UserRole.Equals("Staff", StringComparison.OrdinalIgnoreCase))
                {
                    _context.Staffs.Add(new Staff { UserID = user.Id, User = user });
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // If adding customer/staff fails, delete the user and return error
                await _userManager.DeleteAsync(user);
                return IdentityResult.Failed(new IdentityError { Description = $"Failed to create user role record: {ex.Message}" });
            }
        }

        return result;
    }

    public async Task<string?> LoginAsync(LoginDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            // This calls your token generator logic
            return _jwtTokenService.GenerateToken(user);
        }
        
        return null;
    }
}