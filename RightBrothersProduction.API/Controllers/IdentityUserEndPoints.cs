using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RightBrothersProduction.API.Services;
using RightBrothersProduction.Models;
using RightBrothersProduction.API.DTOs;
using Microsoft.AspNetCore.Http;
using RightBrothersProduction.API.Services.IServices;


namespace RightBrothersProduction.API.Controllers
{
    public static class IdentityUserEndPoints
    {
        public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("auth/signup", CreateUser);

            app.MapPost("auth/signin", SignIn);

            return app;
        }

        public static IEndpointRouteBuilder MapGoogleAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("auth/google", SignInWithGoogle);
            return app;
        }


        [AllowAnonymous]
        private static async Task<IResult> CreateUser([FromBody]UserRegisterationModel
                userRegisterationModel, UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            var existingUser = await userManager.FindByEmailAsync(userRegisterationModel.Email);
            if (existingUser != null)
            {
                return Results.BadRequest(new { errors = new[] { "البريد الإلكتروني مستخدم سابقاً" } });
            }

            ApplicationUser user = new ApplicationUser
            {
                Email = userRegisterationModel.Email,
                FullName = userRegisterationModel.FullName,
                UserName = userRegisterationModel.Email,
                PhoneNumber = userRegisterationModel.PhoneNumber,
                DateJoined = DateTime.UtcNow,
            };
            var result = await userManager.CreateAsync(user, userRegisterationModel.Password);
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
                return await SignInUserAsync(userRegisterationModel.Email, userRegisterationModel.Password, userManager, tokenService);
            } else
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Results.BadRequest(new { errors }); 
            }
         
        }

        [AllowAnonymous]
        private static async Task<IResult> SignInWithGoogle(
            GoogleLoginDto dto,
            HttpContext http,
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IConfiguration config)
        {

            if (dto is null || string.IsNullOrWhiteSpace(dto.idToken))
                return Results.BadRequest("Google ID token is missing");

            // 2️⃣ Validate token with Google
            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(
                    dto.idToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { config["Authentication:Google:ClientId"] },
                        Clock = new SkewedClock(TimeSpan.FromMinutes(2))
                    });
            }
            catch (Exception ex)
            {
                return Results.BadRequest("Invalid Google token.");
            }

            // 3️⃣ Try to find user
            var user = await userManager.Users
                .FirstOrDefaultAsync(u => u.Email == payload.Email);

            var isNew = false;

            // 4️⃣ If not found, create a new user
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    EmailConfirmed = true,
                    FullName = payload.Name
                   
                };

                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return Results.BadRequest(result.Errors);
                }
                await userManager.AddToRoleAsync(user, "User");
                isNew = true;
            }

            // 5️⃣ Issue JWT
            var jwt = await tokenService.CreateTokenAsync(user);

            return Results.Ok(new
            {
                token = jwt,
                isNewUser = isNew,
                email = user.Email,
                fullName = user.FullName
            });
        }

        [AllowAnonymous]
        private static async Task<IResult> SignIn(
        [FromBody] UserLoginModel userLoginModel,
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService)
        {
            return await SignInUserAsync(userLoginModel.Email, userLoginModel.Password, userManager, tokenService);
        }

        public static async Task<IResult> SignInUserAsync(string email, string password, UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null || !await userManager.CheckPasswordAsync(user, password))
                return Results.BadRequest("Invalid email or password.");

            var jwt = await tokenService.CreateTokenAsync(user);

            return Results.Ok(new { token = jwt });

        }


    }
}
