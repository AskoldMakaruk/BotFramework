using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    public interface IClient
    {
        /// <summary>
        ///     Identifier of user. Each user has his/her own <see cref="IClient" />
        /// </summary>
        long UserId { get; }

        Task<TResponse> MakeRequest<TResponse>(
            IRequest<TResponse> request,
            CancellationToken   cancellationToken = default);

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
}