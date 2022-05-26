using BotFramework.Abstractions;
using BotFramework.Extensions;

namespace BotFramework.Services.Commands.Attributes;

public class StartsWithAttribute : CommandAttribute
{
    public readonly string Value;

    public StartsWithAttribute(string value)
    {
        Value = value;
    }

    public override bool? Suitable(UpdateContext context) => context.Update.StartsWith(Value);
}