using Telegram.Bot.Types;

namespace BotFramework.Services.Extensioins
{
    public static class UpdatePredicates
    {
        public static bool StartsWith(this Update update, string value)
        {
            return (update.Message?.Text?.StartsWith(value)             ?? false)
                   || (update.Message?.Caption?.StartsWith(value)       ?? false)
                   || (update.EditedMessage?.Text?.StartsWith(value)    ?? false)
                   || (update.EditedMessage?.Caption?.StartsWith(value) ?? false);
        }
    }
}