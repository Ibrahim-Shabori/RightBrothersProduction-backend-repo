using RightBrothersProduction.Models;

namespace RightBrothersProduction.API.Services.IServices
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(ApplicationUser user);
    }
}
