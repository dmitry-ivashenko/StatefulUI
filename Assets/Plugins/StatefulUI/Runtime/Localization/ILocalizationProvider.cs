namespace StatefulUI.Runtime.Localization
{
    public interface ILocalizationProvider
    {
        public string GetPhrase(string key, string defaultValue);
    }
}