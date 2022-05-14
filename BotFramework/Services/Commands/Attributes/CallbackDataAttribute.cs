using BotFramework.Abstractions;

namespace BotFramework.Services.Commands.Attributes;

public class CallbackDataAttribute : CommandAttribute
{
    private readonly string _callbackData;

    public CallbackDataAttribute(string callbackData)
    {
        _callbackData = callbackData;
    }

    public override bool? Suitable(UpdateContext context)
    {
        return context.Update.CallbackQuery?.Data?.StartsWith(_callbackData);
    }
}