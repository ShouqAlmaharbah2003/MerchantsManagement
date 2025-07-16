using DataLib.NHibernate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using Serilog;
using StackExchange.Redis;
using System.Reflection;

namespace DataLib.Configuration
{
    public static class DataLibSetup
    {
        public static void AddDataLibServices(this IServiceCollection services, IConfiguration configuration)
        {
            // NHibernate
            var connectionString = configuration.GetConnectionString("OracleDb");
            services.AddSingleton<ISessionFactory>(NHibernateHelper.CreateSessionFactory(connectionString));
            services.AddHttpContextAccessor();
            services.AddScoped<ISession>(provider =>
            {
                var sessionFactory = provider.GetRequiredService<ISessionFactory>();
                var session = sessionFactory.OpenSession();
                session.FlushMode = FlushMode.Auto;
                Log.Information("Opened new NHibernate session.");
                return session;
            });

            // Redis Cache
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(
                configuration.GetConnectionString("Redis")));
            services.AddScoped<DataLib.Caching.RedisCacheService>(sp =>
            {
                var redisConnection = configuration.GetConnectionString("Redis");
                return new DataLib.Caching.RedisCacheService(redisConnection);
            });

            //  تسجيل Repositories تلقائيًا
            RegisterRepositories(services);
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            // جلب الـ Assembly الذي يحتوي الـRepositories
            var assembly = Assembly.GetExecutingAssembly();

            // البحث عن كل الكلاسات التي تنتهي بـ "Repository"
            var repositoryTypes = assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.Name.EndsWith("Repository"))
                .ToList();

            foreach (var implementationType in repositoryTypes)
            {
                // البحث عن أول Interface يطبقه الكلاس
                var interfaceType = implementationType.GetInterfaces()
                    .FirstOrDefault(i => i.Name.EndsWith("Repository"));

                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, implementationType);
                    Log.Information("Registered Repository: {Interface} -> {Implementation}",
                        interfaceType.Name, implementationType.Name);
                }
            }
        }
    }
}