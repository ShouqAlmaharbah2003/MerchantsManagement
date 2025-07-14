using System.Reflection;
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
            // ✅ تسجيل الـServices تلقائيًا
            RegisterServices(services);

            // ✅ تسجيل الـJwtSettings
            services.Configure<JwtSettings>(options =>
            {
                var jwtSection = configuration.GetSection("Jwt");
                options.Key = jwtSection["Key"];
                options.Issuer = jwtSection["Issuer"];
                options.Audience = jwtSection["Audience"];
                options.ExpiryInMinutes = int.Parse(jwtSection["ExpiryInMinutes"]);
            });

            // تسجيل IJwtService يدويًا (لأنه Singleton)
            services.AddSingleton<IJwtService, JwtService>();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // جلب الـ Assembly الذي يحتوي الـServices
            var assembly = Assembly.GetExecutingAssembly();

            // البحث عن كل الكلاسات التي تنتهي بـ "Service"
            var serviceTypes = assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.Name.EndsWith("Service"))
                .ToList();

            foreach (var implementationType in serviceTypes)
            {
                // البحث عن أول Interface يطبقه الكلاس
                var interfaceType = implementationType.GetInterfaces()
                    .FirstOrDefault(i => i.Name.EndsWith("Service"));

                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, implementationType);
                    Console.WriteLine($"Registered Service: {interfaceType.Name} -> {implementationType.Name}");
                }
            }
        }
    }
}







//using CommonLib.Interfaces;
//using CommonLib.Services;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;

//namespace CommonLib.Configuration
//{
//    public static class CommonLibSetup
//    {
//        public static void AddCommonLibServices(this IServiceCollection services, IConfiguration configuration)
//        {
//            // Services
//            services.AddScoped<IMerchantsService, MerchantsService>();
//            services.AddScoped<IMerchantBranchesService, MerchantBranchesService>();
//            services.AddScoped<IMerchantGroupsService, MerchantGroupsService>();
//            services.AddScoped<IUserService, UserService>();
//            services.AddScoped<ILocalizationService, LocalizationService>();


//            // JWT Settings
//            services.Configure<JwtSettings>(options =>
//            {
//                var jwtSection = configuration.GetSection("Jwt");
//                options.Key = jwtSection["Key"];
//                options.Issuer = jwtSection["Issuer"];
//                options.Audience = jwtSection["Audience"];
//                options.ExpiryInMinutes = int.Parse(jwtSection["ExpiryInMinutes"]);
//            });
//            services.AddSingleton<IJwtService, JwtService>();
//        }
//    }
//}