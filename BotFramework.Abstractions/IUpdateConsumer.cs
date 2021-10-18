using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotFramework.Abstractions
{
    public interface IUpdateConsumer : IClient
    {
        bool IsDone { get; }
        
        void Consume(Update update);

        void Initialize(Func<IClient, Task> command, Update update);
    }
}