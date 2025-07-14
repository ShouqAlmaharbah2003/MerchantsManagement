using System.Globalization;
using System.Resources;

namespace CommonLib.Utils
{
    public static class LocalizationHelper
    {
        public static string GetLocalizedString(string key, string culture)
        {
            var resourceManager = new ResourceManager("CommonLib.Resources.Resources", typeof(LocalizationHelper).Assembly);
            var ci = new CultureInfo(culture);
            return resourceManager.GetString(key, ci);
        }
    }
}