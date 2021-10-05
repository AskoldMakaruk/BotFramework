using Telegram.Bot.Types;

namespace BotFramework.Abstractions
{
    public interface IUpdateConsumer : IClient
    {
        bool IsDone             { get; }
        bool IsWaitingForUpdate { get; }
        void Consume(Update update);

        void Initialize(ICommand command, Update update);
    }
}