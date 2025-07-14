using CommonLib.Interfaces;
using CommonLib.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommonLib.Configuration
{
    public static class CommonLibSetup
    {
        public static void AddCommonLibServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Services
            services.AddScoped<IMerchantsService, MerchantsService>();
            services.AddScoped<IMerchantBranchesService, MerchantBranchesService>();
            services.AddScoped<IMerchantGroupsService, MerchantGroupsService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILocalizationService, LocalizationService>();


            // JWT Settings
            services.Configure<JwtSettings>(options =>
            {
                var jwtSection = configuration.GetSection("Jwt");
                options.Key = jwtSection["Key"];
                options.Issuer = jwtSection["Issuer"];
                options.Audience = jwtSection["Audience"];
                options.ExpiryInMinutes = int.Parse(jwtSection["ExpiryInMinutes"]);
            });
            services.AddSingleton<IJwtService, JwtService>();
        }
    }
}
