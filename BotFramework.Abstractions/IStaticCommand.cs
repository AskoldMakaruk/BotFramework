using Telegram.Bot.Types;

namespace BotFramework.Abstractions
{
    public interface IStaticCommand : ICommand
    {
        /// <summary>
        ///     If returns <c>true</c>, instance of this command will be created, executed and any other <see cref="ICommand" />
        ///     discarded, even current command.
        /// </summary>
        /// <remarks>
        ///     It must not use any non-static context.
        /// </remarks>
        bool SuitableFirst(Update update) => false;

        /// <summary>
        ///     If there is no current command nor any <see cref="SuitableFirst" /> command and this returns <c>true</c>, instance
        ///     of this command will be created and executed. Any other <see cref="SuitableLast" /> command will be discarded.
        /// </summary>
        /// <remarks>
        ///     It must not use any non-static context.
        /// </remarks>
        bool SuitableLast(Update update) => false;
    }
}