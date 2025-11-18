

using RightBrothersProduction.API.Controllers;
using RightBrothersProduction.API.Extensions;
using RightBrothersProduction.API.Services;
using RightBrothersProduction.DataAccess.Repositories;
using RightBrothersProduction.DataAccess.Repositories.IRepositories;


namespace RightBrothersProduction.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle



            builder.Services
                .AddSwaggerExplorer()
                .InjectDbContext(builder.Configuration)
                .AddAppConfiguration(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .ConfigureIdentityOptions()
                .AddIdentityAuth(builder.Configuration);

            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


            var app = builder.Build();


            await app.SeedIdentityAsync();

            // Configure the HTTP request pipeline.
            app.ConfigureSwaggerExplorer()
                .ConfigureCORS(builder.Configuration)
                .AddIdentityAuthMiddlewares();


            app.MapControllers();

            app.MapGroup("/api")
                .MapIdentityUserEndpoints()
                .MapGoogleAuthEndpoints();

            app.Run();
        }
    }
}
