using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Vehicle.Application.Interface.IServices;
using Vehicle.Domain.Models;

namespace Vehicle.Infrastructure.Service;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _cfg;
    private readonly UserManager<ApplicationUser> _userMgr;

    public JwtTokenService(IConfiguration cfg, UserManager<ApplicationUser> userMgr)
    {
        _cfg = cfg;
        _userMgr = userMgr;
    }

    public string GenerateToken(ApplicationUser user)
    {
        // sync wrapper for interface; safe — UserManager call is fast and we only do it on login.
        var roles = _userMgr.GetRolesAsync(user).GetAwaiter().GetResult();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new("FullName", user.FullName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // Use Identity roles. Fall back to UserRole field if no roles assigned yet.
        if (roles.Count == 0 && !string.IsNullOrWhiteSpace(user.UserRole))
            claims.Add(new Claim(ClaimTypes.Role, user.UserRole));
        else
            foreach (var r in roles) claims.Add(new Claim(ClaimTypes.Role, r));

        // marker so we know this token uses Identity roles
        claims.Add(new Claim("IdentityRoleClaims", "true"));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var hours = int.TryParse(_cfg["Jwt:ExpiryHours"], out var h) ? h : 1;

        var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            audience: _cfg["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(hours),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
