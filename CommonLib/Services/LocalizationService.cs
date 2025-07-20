//using Microsoft.Extensions.Localization;
//using CommonLib.Interfaces;
//using CommonLib.Resources;

//namespace CommonLib.Services
//{
//    public class LocalizationService : ILocalizationService
//    {
//        private readonly IStringLocalizer _localizer;

//        public LocalizationService(IStringLocalizerFactory factory)
//        {
//            // Fix: Use the correct resource type, not the namespace.
//            _localizer = factory.Create(typeof(CommonLib.Resources.Resources));
//        }

//        public string GetLocalizedString(string key)
//        {
//            return _localizer[key];
//        }
//    }
//}