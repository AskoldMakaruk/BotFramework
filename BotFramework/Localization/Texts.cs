using BotFramework.Identity;

namespace BotFramework.Localization;

public class Texts
{
    private readonly IdentityUser         _user;
    private readonly TranslationContainer _container;
    
    public           string?              LocaleCode => _user?.LanguageCode;

    public string this[string key] => LocaleCode == null ? key : _container[LocaleCode][key];

    public Texts(IdentityUser user, TranslationContainer container)
    {
        _user      = user;
        _container = container;
    }
}