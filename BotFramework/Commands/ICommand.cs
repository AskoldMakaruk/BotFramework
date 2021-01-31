using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Responses;
using Telegram.Bot.Types;

namespace BotFramework.Commands
{
    public interface ICommand
    {
        Task<Response> Execute(IClient client);
    }

    public abstract class StaticCommand : ICommand
    {
        /// <summary>
        /// If returns <c>true</c>, instance of this command will be created, executed and any other <see cref="ICommand"/> discarded, even current command.
        /// </summary>
        /// <remarks>
        /// It must not use any non-static context.
        /// </remarks>
        public virtual bool SuitableFirst(Update message) => false;

        /// <summary>
        /// If there is no current command nor any <see cref="SuitableFirst"/> command and this returns <c>true</c>, instance of this command will be created and executed. Any other <see cref="SuitableLast"/> command will be discarded.
        /// </summary>
        /// <remarks>
        /// It must not use any non-static context.
        /// </remarks>
        public virtual bool SuitableLast(Update message) => false;

        public abstract Task<Response> Execute(IClient client);
    }
}