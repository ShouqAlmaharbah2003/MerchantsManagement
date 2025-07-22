using CommonLib.Resources;
using Microsoft.Extensions.Localization;

namespace CommonLib.Utils
{
    public static class LocalizationHelper
    {

        private static IStringLocalizer<SharedResources> _localizer;    

        public static void Configure(IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
        }


        public static string GetLocalizedString(string key, string culture)
        {
            var message = _localizer[key];
            return message;
        }
    }
}