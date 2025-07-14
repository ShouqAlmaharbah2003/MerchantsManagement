using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.Globalization;

namespace ApiLib.Filters
{
    public class LocalizationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var culture = context.HttpContext.Request.Headers["Accept-Language"].FirstOrDefault();
                if (!string.IsNullOrEmpty(culture))
                {
                    var primaryCulture = culture.Split(';').First().Split(',').First().Trim();
                    Log.Information("Setting culture to: {Culture} from Accept-Language header", primaryCulture);
                    CultureInfo.CurrentCulture = new CultureInfo(primaryCulture);
                    CultureInfo.CurrentUICulture = new CultureInfo(primaryCulture);
                }
                else
                {
                    Log.Information("No Accept-Language header found, using default culture: {DefaultCulture}", CultureInfo.CurrentCulture.Name);
                }
            }
            catch (CultureNotFoundException ex)
            {
                Log.Error(ex, "Invalid culture specified in Accept-Language header. Defaulting to {DefaultCulture}. Exception: {Message}",
                    "en-US", ex.Message);
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
                CultureInfo.CurrentUICulture = new CultureInfo("en-US");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during culture setup. Defaulting to {DefaultCulture}. Exception: {Message}",
                    "en-US", ex.Message);
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
                CultureInfo.CurrentUICulture = new CultureInfo("en-US");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { /* not needed */ }
    }
}