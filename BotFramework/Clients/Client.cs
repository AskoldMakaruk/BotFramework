using System;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Helpers;
using Serilog;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    /// <inheritdoc cref="IClient" />
    /// >
    public class Client : IUpdateConsumer
    {
        private readonly UpdateHandler _updateHandler;
        private readonly ILogger       _logger;
        private readonly IRequestSinc  _client;

        private Func<Update, bool>? CurrentFilter;
        private Action<Update>?     OnFilterFail;

        public long UserId             { get; private set; }
        public bool IsDone             => _updateHandler.IsDone;

        public Client(IRequestSinc client, UpdateHandler updateHandler, ILogger logger)
        {
            _client        = client;
            _updateHandler = updateHandler;
            _logger        = logger;
        }

        public void Initialize(ICommand command, Update update)
        {
            UserId = (long)update.GetId()!;
            Consume(update);
            _updateHandler.CurrentTask = command.Execute(this);
        }

        public ValueTask<Update> GetUpdate(Func<Update, bool>? filter = null, Action<Update>? onFilterFail = null)
        {
            CurrentFilter = filter;
            OnFilterFail  = onFilterFail;

            return _updateHandler.GetUpdate(filter, onFilterFail);
        }

        public async Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request,
                                                            CancellationToken   cancellationToken = default)
        {
            _logger.Verbose("{Message}", await request.ToHttpContent().ReadAsStringAsync(cancellationToken));
            return await _client.MakeRequest(request, cancellationToken);
        }

        public void Consume(Update update)
        {
            _updateHandler.Consume(update, CurrentFilter, OnFilterFail);
        }
    }
}