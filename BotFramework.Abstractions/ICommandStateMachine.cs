using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotFramework.Abstractions
{
    public interface IUpdateQueue
    {
        /// <summary>
        ///     Get next <see cref="Update" /> for user with this <see cref="UserId" />.
        ///     If invoked in <see cref="IStaticCommand" /> first <see cref="Update" /> will be the same as in
        ///     <see cref="IStaticCommand.SuitableFirst" /> or <see cref="IStaticCommand.SuitableLast" /> methods.
        /// </summary>
        /// <param name="filter">
        ///     If not <c>null</c>, returned <see cref="ValueTask" /> will be completed only when incoming <see cref="Update" />
        ///     will satisfy <see cref="filter" /> predicate.
        /// </param>
        /// <param name="onFilterFail">
        ///     For every incoming <see cref="Update" />, if <see cref="filter" /> return <c>false</c>, this <see cref="Action" />
        ///     will be invoked
        /// </param>
        /// <returns>
        ///     A <see cref="ValueTask" /> with <see cref="Update" /> related to user with this <see cref="UserId" />
        /// </returns>
        ValueTask<Update> GetUpdate(Func<Update, bool>? filter = null, Action<Update>? onFilterFail = null);
    }

    /// <summary>
    /// Consumer for one command. Only one ICommandStateMachine can handle some update at same time
    /// </summary>
    public interface ICommandStateMachine : IUpdateQueue
    {
        bool IsDone { get; }

        void Consume(Update update);

        void Initialize(Task command);
    }
}