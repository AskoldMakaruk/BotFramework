using BotFramework.Abstractions;
using BotFramework.Services.Extensioins;

namespace BotFramework.Services.Controllers.Attributes;

public class StartsWithAttribute : CommandAttribute
{
    private readonly string _value;

    public StartsWithAttribute(string value)
    {
        _value = value;
    }

    public override bool? Suitable(UpdateContext context) => context.Update.StartsWith(_value);
}