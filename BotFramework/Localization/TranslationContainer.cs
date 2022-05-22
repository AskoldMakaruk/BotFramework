using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BotFramework.Localization;

public class TranslationContainer
{
    public ConcurrentDictionary<string, TranlsationLanguage> Languages { get; }

    public TranlsationLanguage this[string input]
    {
        get
        {
            if (!Languages.ContainsKey(input))
            {
                Languages[input] = new TranlsationLanguage(input);
            }

            return Languages[input];
        }
    }

    internal TranslationContainer(IReadOnlyDictionary<string, TranlsationLanguage> languages)
    {
        Languages = new ConcurrentDictionary<string, TranlsationLanguage>(languages);
    }
}