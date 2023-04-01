using System;
using System.Collections.Generic;
using StatefulUI.Runtime.Core;

namespace StatefulUI.Runtime.Localization
{
    public static class LocalizationUtils
    {
        private static ILocalizationProvider _localizationProvider;
        
        public static string GetPhrase(string key, string defaultValue)
        {
            _localizationProvider ??= FindLocalizationProvider() ?? new EmptyLocalizationProvider();
            return _localizationProvider.GetPhrase(key, defaultValue);
        }

        private static ILocalizationProvider FindLocalizationProvider()
        {
            return StatefulUiUtils.FindImplementation<ILocalizationProvider>(typeof(EmptyLocalizationProvider));
        }
    }
}
