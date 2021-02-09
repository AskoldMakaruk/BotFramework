using Telegram.Bot.Types;

namespace BotFramework.Abstractions
{
    public interface IUpdateConsumer
    {
        bool IsDone             { get; }
        bool IsWaitingForUpdate { get; }
        void Consume(Update update);
    }
}