using System.ComponentModel.DataAnnotations;

namespace BotFramework.Localization;

public class BotFrameworkLocale
{
    [Key]
    public string LocaleCode { get; set; }
}