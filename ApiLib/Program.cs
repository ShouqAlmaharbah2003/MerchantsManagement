using ApiLib.Configuration;
using CommonLib.Configuration;
using CommonLib.Resources;
using CommonLib.Utils;
using DataLib.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
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
//app.UseRequestLocalization();
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

var localizer = app.Services.GetRequiredService<IStringLocalizer<SharedResources>>();
LocalizationHelper.Configure(localizer);

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