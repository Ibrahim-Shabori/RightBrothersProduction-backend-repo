using RightBrothersProduction.Models;

namespace RightBrothersProduction.API.Extensions
{
    public static class AppConfigurationExtensions
    {
        public static WebApplication ConfigureCORS(this WebApplication app, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();

            app.UseCors(policy =>
            {
                if (allowedOrigins != null && allowedOrigins.Length > 0)
                {
                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            });

            return app;
        }

        public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            return services;
        }
    }
}
