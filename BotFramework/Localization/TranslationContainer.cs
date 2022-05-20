using System.Collections.Generic;

namespace BotFramework.Localization;

public class TranslationContainer
{
    public IReadOnlyDictionary<string, TranlsationLanguage> Languages { get; }
    public TranlsationLanguage this[string input] => Languages[input];

    internal TranslationContainer(IReadOnlyDictionary<string, TranlsationLanguage> languages)
    {
        Languages = languages;
    }
}