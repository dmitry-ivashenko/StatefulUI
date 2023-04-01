namespace StatefulUI.Runtime.Localization
{
    public class EmptyLocalizationProvider : ILocalizationProvider
    {
        public string GetPhrase(string key, string defaultValue)
        {
            return defaultValue;
        }
    }
}