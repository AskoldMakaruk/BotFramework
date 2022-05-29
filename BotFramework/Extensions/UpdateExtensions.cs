using Telegram.Bot.Types;

namespace BotFramework.Extensions;

public static class UpdateExtensions
{
    public static bool StartsWith(this Update update, string value)
    {
        return update.GetText()?.StartsWith(value) ?? false;
    }

    public static bool EndsWith(this Update update, string value)
    {
        return update.GetText()?.EndsWith(value) ?? false;
    }

    public static string? GetText(this Update update)
    {
        return update.Message?.Text
               ?? update.EditedMessage?.Text
               ?? update.Message?.Caption
               ?? update.EditedMessage?.Caption;
    }
}