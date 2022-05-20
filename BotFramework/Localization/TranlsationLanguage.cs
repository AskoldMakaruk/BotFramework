using System.Collections.Concurrent;

namespace BotFramework.Localization;

public class TranlsationLanguage
{
    public string                               Locale;
    public ConcurrentDictionary<string, string> Translations = new();

    public TranlsationLanguage(string locale)
    {
        Locale = locale;
    }

    public string this[string input]
    {
        get
        {
            if (Translations.ContainsKey(input))
            {
                return Translations[input];
            }

            this[input] = input;

            return input;
        }
        private set => Translations[input] = value;
    }
}