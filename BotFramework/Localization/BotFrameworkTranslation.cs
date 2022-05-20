using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotFramework.Localization;

public class BotFrameworkTranslation
{
    [Key]
    public int Id { get; set; }

    public string KeyName { get; set; }
    public string Value   { get; set; }

    [ForeignKey("Locale")]
    public string LocaleCode { get; set; }

    public BotFrameworkLocale Locale { get; set; }
}