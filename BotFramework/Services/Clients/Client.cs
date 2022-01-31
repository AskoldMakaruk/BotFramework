using System;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Services.Clients
{
    /// <inheritdoc cref="IClient"/>
    public class Client : IClient
    {
        public long UserId { get; }

        private readonly IRequestSinc _requestSinc;
        private readonly IUpdateQueue _updateQueue;

        public Client(IRequestSinc requestSinc, ICommandStateMachine updateQueue, Update? update)        
        {
            UserId       = update?.GetId() ?? -1;
            _requestSinc = requestSinc;
            _updateQueue = updateQueue;
        }

        public async Task<TResponse> MakeRequest<TResponse>(
            IRequest<TResponse> request,
            CancellationToken   cancellationToken = default)
        {
            return await _requestSinc.MakeRequest(request, cancellationToken);
        }

        public ValueTask<Update> GetUpdate(Func<Update, bool>? filter = null, Action<Update>? onFilterFail = null)
        {
            return _updateQueue.GetUpdate(filter, onFilterFail);
        }
    }
}