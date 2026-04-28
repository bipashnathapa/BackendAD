using Vehicle.Domain.Models;

namespace Vehicle.Application.Interface.IServices;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user);
}