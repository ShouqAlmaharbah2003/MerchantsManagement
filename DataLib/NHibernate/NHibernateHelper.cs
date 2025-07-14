using DataLib.NHibernate.Mappings;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using Serilog;

namespace DataLib.NHibernate
{
    public static class NHibernateHelper
    {
        public static ISessionFactory CreateSessionFactory(string connectionString)
        {
            try
            {
                var configuration = Fluently.Configure()
                    .Database(OracleManagedDataClientConfiguration.Oracle10
                        .ConnectionString(c => c.Is(connectionString))
                        .Dialect<Oracle10gDialect>()
                        .Driver<OracleManagedDataClientDriver>()
                        .ShowSql()
                    )
                    .Mappings(m =>
                    {
                        m.FluentMappings.AddFromAssemblyOf<UserMap>();
                        m.FluentMappings.AddFromAssemblyOf<MerchantMap>();
                        m.FluentMappings.AddFromAssemblyOf<MerchantBranchMap>();
                        m.FluentMappings.AddFromAssemblyOf<MerchantGroupMap>();
                        m.FluentMappings.AddFromAssemblyOf<LoginTokenMap>();
                    })
                    .ExposeConfiguration(cfg =>
                    {
                        // استخدام "call" بدلاً من "web" لدعم ASP.NET Core
                        cfg.SetProperty("current_session_context_class", "call");

                        var executeSchema = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
                        Log.Information("Executing SchemaUpdate: {ExecuteSchema}", executeSchema);
                        new SchemaUpdate(cfg).Execute(useStdOut: true, doUpdate: executeSchema);

                        new SchemaExport(cfg)
                            .SetOutputFile("schema.sql")
                            .SetDelimiter(";")
                            .Create(useStdOut: true, execute: false);
                    });

                Log.Information("Building NHibernate session factory...");
                var sessionFactory = configuration.BuildSessionFactory();
                Log.Information("NHibernate session factory created successfully.");
                return sessionFactory;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create NHibernate session factory.");
                throw;
            }
        }
    }
}