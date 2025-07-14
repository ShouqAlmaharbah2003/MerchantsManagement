using Microsoft.Extensions.Localization;
using CommonLib.Interfaces;

namespace CommonLib.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizer _localizer;

        public LocalizationService(IStringLocalizerFactory factory)
        {
            var type = typeof(LocalizationService);
            _localizer = factory.Create(type);
        }

        public string GetLocalizedString(string key)
        {
            return _localizer[key];
        }
    }
}