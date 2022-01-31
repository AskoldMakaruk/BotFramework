using BotFramework.Services.Extensioins;
using Telegram.Bot.Types;

namespace BotFramework.Services.Controllers.Attributes;

public class StartsWithAttribute : CommandAttribute
{
    private readonly string _value;

    public StartsWithAttribute(string value)
    {
        _value = value;
    }

    public override bool? Suitable(Update context) => context.StartsWith(_value);
}