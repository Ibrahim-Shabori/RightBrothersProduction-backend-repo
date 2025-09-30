using Microsoft.EntityFrameworkCore;
using RightBrothersProduction.DataAccess.Data;
using RightBrothersProduction.DataAccess.Seed;

namespace RightBrothersProduction.API.Extensions
{
    public static class AppBuilderExtensions
    {
        public static async Task SeedIdentityAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync();

            await IdentitySeeder.SeedRolesAndAdminAsync(scope.ServiceProvider);
        }
    }
}
