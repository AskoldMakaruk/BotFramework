using Telegram.Bot.Types;

namespace BotFramework.Abstractions
{
    public interface IBotContext
    {
        User ChatId { get; }
        Update CurrentUpdate { get; }
    }
}