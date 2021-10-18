using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Extensions;
using Serilog;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    public class Client : IUpdateConsumer // todo delete this class its just a proxy
    {
        private readonly ILogger      _logger;
        private readonly IRequestSinc _client;


        public           long                                      UserId { get; private set; }
        private readonly ConcurrentDictionary<UpdateHandler, bool> _handlers = new();
        private readonly ConcurrentQueue<Update>                   Updates   = new();
        public           bool                                      IsDone => CurrentTask.IsCompleted;
        private          Task                                      CurrentTask = null!;

        public Client(IRequestSinc client, ILogger logger)
        {
            _client = client;
            _logger = logger;
        }

        //should move initialize in another class/interface maybe
        public void Initialize(Func<IClient, Task> command, Update update)
        {
            UserId = (long)update.GetId()!;
            Consume(update);
            CurrentTask = command(this);
        }

        public ValueTask<Update> GetUpdate(Func<Update, bool>? filter = null, Action<Update>? onFilterFail = null)
        {
            var handler = new UpdateHandler(filter, onFilterFail, Updates, handler => _handlers.TryRemove(handler, out _));
            _handlers.TryAdd(handler, default);
            return handler.GetUpdate();
        }

        public async Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request,
                                                            CancellationToken   cancellationToken = default)
        {
            _logger.Verbose("{Message}", await request.ToHttpContent().ReadAsStringAsync(cancellationToken));
            return await _client.MakeRequest(request, cancellationToken);
        }

        public void Consume(Update update)
        {
            foreach (var (handler, _) in _handlers)
                handler.HandleUpdate(update);
            Updates.Enqueue(update);
        }
    }
}