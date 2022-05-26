using System;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Services.Clients;

/// <inheritdoc cref="IClient"/>
public class Client : IClient
{
    public long   UserId { get; }
    public Update Update { get; private set; }

    private readonly IRequestSinc _requestSinc;
    private readonly IUpdateQueue _updateQueue;

    public Client(IRequestSinc requestSinc, ICommandStateMachine updateQueue, Update? update)
    {
        UserId       = update?.GetId() ?? -1;
        _requestSinc = requestSinc;
        _updateQueue = updateQueue;
        Update       = update!;
    }

    public async Task<TResponse> MakeRequest<TResponse>(
        IRequest<TResponse> request,
        CancellationToken   cancellationToken = default)
    {
        return await _requestSinc.MakeRequest(request, cancellationToken);
    }

    public async ValueTask<Update> GetUpdate(Func<Update, bool>? filter = null, Action<Update>? onFilterFail = null)
    {
        var update = await _updateQueue.GetUpdate(filter, onFilterFail);
        Update = update;
        return update;
    }
}