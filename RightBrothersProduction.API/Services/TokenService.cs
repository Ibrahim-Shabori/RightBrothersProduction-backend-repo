using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RightBrothersProduction.API.Services.IServices;
using RightBrothersProduction.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RightBrothersProduction.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(IOptions<AppSettings> appSettings,
                            UserManager<ApplicationUser> userManager)
        {
            _appSettings = appSettings;
            _userManager = userManager;
        }

        public async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("fullName", user.FullName),
            new Claim("profilePictureUrl", user.ProfilePictureUrl)
            };

            // add role claims
            claims.AddRange(roles.Select(r => new Claim("role", r)));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_appSettings.Value.JWTSecret!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
