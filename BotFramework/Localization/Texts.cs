using BotFramework.Identity;

namespace BotFramework.Localization;

public class Texts
{
    private readonly TranslationContainer _container;
    public readonly  string?              LocaleCode;

    public string this[string key] => LocaleCode == null ? key : _container[LocaleCode][key];

    public Texts(IdentityUser user, TranslationContainer container)
    {
        _container = container;
        LocaleCode = user.LanguageCode;
    }
}