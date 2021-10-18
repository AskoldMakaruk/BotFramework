using System;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Extensions;
using Serilog;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    /// <inheritdoc cref="IClient" />
    /// >
    public class Client : IUpdateConsumer // todo delete this class its just a proxy
    {
        private readonly UpdateQueue  _updateQueue;
        private readonly ILogger      _logger;
        private readonly IRequestSinc _client;

        private Func<Update, bool>? CurrentFilter;
        private Action<Update>?     OnFilterFail;
        private Task                CurrentTask;

        public long UserId { get; private set; }
        public bool IsDone => CurrentTask.IsCompleted;

        public Client(IRequestSinc client, UpdateQueue updateQueue, ILogger logger)
        {
            _client      = client;
            _updateQueue = updateQueue;
            _logger      = logger;
        }

        public void Initialize(Func<IClient, Task> command, Update update)
        {
            UserId = (long)update.GetId()!;
            Consume(update);
            CurrentTask = command(this);
        }

        public ValueTask<Update> GetUpdate(Func<Update, bool>? filter = null, Action<Update>? onFilterFail = null)
        {
            CurrentFilter = filter;
            OnFilterFail  = onFilterFail;

            return _updateQueue.GetUpdate(filter, onFilterFail);
        }

        public async Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request,
                                                            CancellationToken   cancellationToken = default)
        {
            _logger.Verbose("{Message}", await request.ToHttpContent().ReadAsStringAsync(cancellationToken));
            return await _client.MakeRequest(request, cancellationToken);
        }

        public void Consume(Update update)
        {
            _updateQueue.Consume(update, CurrentFilter, OnFilterFail);
        }
    }
}