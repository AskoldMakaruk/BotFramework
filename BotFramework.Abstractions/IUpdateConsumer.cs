using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotFramework.Abstractions
{
    /// <summary>
    /// Consumer for one command. Only one IUpdateConsumer can handle some update at same time
    /// </summary>
    public interface IUpdateConsumer : IClient
    {
        bool IsDone { get; }
        
        void Consume(Update update);

        void Initialize(Func<IClient, Task> command, Update update);
    }
}