using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using RightBrothersProduction.Models;
using System.Security.Claims;

namespace RightBrothersProduction.API.Controllers
{
    public static class AccountEndPoints
    {
        public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/UserProfile", GetUserProfile);
            return app;
        }

        [Authorize]
        private static async Task<IResult> GetUserProfile(
            ClaimsPrincipal user, UserManager<ApplicationUser> userManager)
        {
            var userId = user.Claims.First(x => x.Type == "UserId")?.Value;
            var userDetails = await userManager.FindByIdAsync(userId!);
            if (userDetails == null)
            {
                return Results.NotFound("User not found");
            }

            return Results.Ok(new {
                Email = userDetails.Email,
                FullName = userDetails.FullName,

            });
        }
        
    }
}
