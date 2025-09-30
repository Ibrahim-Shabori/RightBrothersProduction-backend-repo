using RightBrothersProduction.Models;

namespace RightBrothersProduction.API.Services
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(ApplicationUser user);
    }
}
