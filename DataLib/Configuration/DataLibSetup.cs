using DataLib.Interfaces;
using DataLib.NHibernate;
using DataLib.Repositories;
using NHibernate;
using Serilog;
using StackExchange.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

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

            // Repositories
            services.AddScoped<IMerchantsRepository, MerchantsRepository>();
            services.AddScoped<IMerchantBranchesRepository, MerchantBranchesRepository>();
            services.AddScoped<IMerchantGroupsRepository, MerchantGroupsRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
