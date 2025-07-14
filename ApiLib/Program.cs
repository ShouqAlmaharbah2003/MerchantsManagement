using ApiLib.Configuration;
using CommonLib.Configuration;
using DataLib.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------
// Logging
// ------------------------------
builder.Host.UseSerilog((context, config) =>
{
    config.WriteTo.Console()
          .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day);
});

builder.Services.AddLogging(logging => logging.AddSerilog());

// ------------------------------
// Setup Projects
// ------------------------------
builder.Services.AddDataLibServices(builder.Configuration);
builder.Services.AddCommonLibServices(builder.Configuration);
builder.Services.AddApiLibServices(builder.Configuration);

var app = builder.Build();

// ------------------------------
// Middleware & Routing
// ------------------------------
app.UseRequestLocalization();
app.UseMiddleware<ApiLib.Middleware.LoggingMiddleware>();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");

app.MapControllers();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
































//using ApiLib.Filters;
//using ApiLib.Middleware;
//using CommonLib.Configuration;
//using CommonLib.Interfaces;
//using CommonLib.Services;
//using DataLib.Interfaces;
//using DataLib.NHibernate;
//using DataLib.Repositories;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Diagnostics;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using NHibernate;
//using Serilog;
//using StackExchange.Redis;
//using System.Text;
//using ISession = NHibernate.ISession;
//using Microsoft.AspNetCore.Http;

//var builder = WebApplication.CreateBuilder(args);

//// ------------------------------
//// Serilog Logging
//// ------------------------------
//Log.Logger = new LoggerConfiguration()
//    .WriteTo.Console()
//    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
//    .CreateLogger();
//builder.Host.UseSerilog();

//// ------------------------------
//// Connection String & NHibernate
//// ------------------------------
////var connectionString = builder.Configuration.GetConnectionString("OracleDb");
////builder.Services.AddSingleton<ISessionFactory>(NHibernateHelper.CreateSessionFactory(connectionString));

////builder.Services.AddHttpContextAccessor();
////builder.Services.AddScoped<ISession>(provider =>
////{
////    var sessionFactory = provider.GetRequiredService<ISessionFactory>();
////    var session = sessionFactory.OpenSession();
////    session.FlushMode = FlushMode.Auto;
////    Log.Information("Opened new NHibernate session.");
////    return session;
////});

//// ------------------------------
//// Redis Cache
//// ------------------------------
////builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(
////    builder.Configuration.GetConnectionString("Redis")));
////builder.Services.AddScoped<DataLib.Caching.RedisCacheService>(sp =>
////{
////    var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("Redis");
////    return new DataLib.Caching.RedisCacheService(connectionString);
////});

//// ------------------------------
//// Repositories
//// ------------------------------
////builder.Services.AddScoped<IMerchantsRepository, MerchantsRepository>();
////builder.Services.AddScoped<IMerchantBranchesRepository, MerchantBranchesRepository>();
////builder.Services.AddScoped<IMerchantGroupsRepository, MerchantGroupsRepository>();
////builder.Services.AddScoped<IUserRepository, UserRepository>();

//// ------------------------------
//// Services
//// ------------------------------
//builder.Services.AddScoped<IMerchantsService, MerchantsService>();
//builder.Services.AddScoped<IMerchantBranchesService, MerchantBranchesService>();
//builder.Services.AddScoped<IMerchantGroupsService, MerchantGroupsService>();
//builder.Services.AddScoped<IUserService, UserService>();

//builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
//builder.Services.AddSingleton<IJwtService, JwtService>();

//// ------------------------------
//// Filters
//// ------------------------------
//builder.Services.AddScoped<AuthorizationFilter>();
//builder.Services.AddScoped<LocalizationFilter>();

//// ------------------------------
//// JWT Authentication
//// ------------------------------
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience = builder.Configuration["Jwt:Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(key),
//        ClockSkew = TimeSpan.Zero
//    };
//});

//// ------------------------------
//// Logging
//// ------------------------------
//builder.Services.AddLogging(logging => logging.AddSerilog());

//// ------------------------------
//// Localization
//// ------------------------------
//builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
//builder.Services.AddControllers()
//    .AddViewLocalization()
//    .AddDataAnnotationsLocalization();

//builder.Services.Configure<RequestLocalizationOptions>(options =>
//{
//    var supportedCultures = new[] { "en-US", "ar-SA" };
//    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
//    options.SupportedCultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToArray();
//    options.SupportedUICultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToArray();
//});

//// ------------------------------
//// CORS
//// ------------------------------
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});

//// ------------------------------
//// Controllers & Swagger
//// ------------------------------
//builder.Services.AddControllers();

//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new() { Title = "Merchants Management API", Version = "v1" });
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        Scheme = "bearer",
//        BearerFormat = "Jwt",
//        In = ParameterLocation.Header,
//        Description = "Enter 'your token'"
//    });
//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            new string[] {}
//        }
//    });
//});

//var app = builder.Build();

//// ------------------------------
//// Middleware
//// ------------------------------
//app.UseExceptionHandler(errorApp =>
//{
//    errorApp.Run(async context =>
//    {
//        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
//        if (exceptionHandlerPathFeature?.Error != null)
//        {
//            Log.Error(exceptionHandlerPathFeature.Error, "Unhandled exception occurred at {Path}", context.Request.Path);
//            await context.Response.WriteAsJsonAsync(new
//            {
//                Success = false,
//                Message = "An unexpected error occurred.",
//                Error = exceptionHandlerPathFeature.Error.Message
//            });
//        }
//    });
//});

//app.UseMiddleware<LoggingMiddleware>();

//app.UseRouting();

//app.UseRequestLocalization();

//app.UseSwagger();
//app.UseSwaggerUI();

//app.UseCors("AllowAll");

//app.UseHttpsRedirection();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//app.Run();