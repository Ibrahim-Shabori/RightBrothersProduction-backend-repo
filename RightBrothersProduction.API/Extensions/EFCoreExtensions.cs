using Microsoft.EntityFrameworkCore;
using RightBrothersProduction.DataAccess.Data;

namespace RightBrothersProduction.API.Extensions
{
    public static class EFCoreExtensions
    {

        public static IServiceCollection InjectDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DevConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure()
                ));
            return services;
        }
    }
}
