using System.ComponentModel.DataAnnotations;

namespace BotFramework.Localization;

public class BotFrameworkLocale
{
    [Key]
    public string LocaleCode { get; set; }

    public static implicit operator string(BotFrameworkLocale locale)
    {
        return locale.LocaleCode;
    }

    public static implicit operator BotFrameworkLocale(string locale)
    {
        return new BotFrameworkLocale() { LocaleCode = locale };
    }
}