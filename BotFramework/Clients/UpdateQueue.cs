using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Extensions;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    public class Client : IClient
    {
        public long UserId { get; }

        private readonly IRequestSinc _requestSinc;
        private readonly IUpdateQueue _updateQueue;

        public Client(IRequestSinc requestSinc, ICommandUpdateConsumer updateQueue, Update update)
        {
            UserId       = update.GetId()!.Value;
            _requestSinc = requestSinc;
            _updateQueue = updateQueue;
        }

        public Task<TResponse> MakeRequest<TResponse>(
            IRequest<TResponse> request,
            CancellationToken   cancellationToken = default)
        {
            return _requestSinc.MakeRequest(request, cancellationToken);
        }

        public ValueTask<Update> GetUpdate(Func<Update, bool>? filter = null, Action<Update>? onFilterFail = null)
        {
            return _updateQueue.GetUpdate(filter, onFilterFail);
        }
    }

    public class CommandUpdateConsumer : ICommandUpdateConsumer
    {
        public void Initialize(Task command)
        {
            CurrentTask = command;
        }

        private readonly ConcurrentDictionary<UpdateTask, bool> _handlers = new();
        private readonly ConcurrentQueue<Update>                Updates   = new();
        public           bool                                   IsDone => CurrentTask.IsCompleted;
        private          Task                                   CurrentTask = null!;

        public ValueTask<Update> GetUpdate(Func<Update, bool>? filter = null, Action<Update>? onFilterFail = null)
        {
            var handler = new UpdateTask(filter, onFilterFail, Updates, handler => _handlers.TryRemove(handler, out _));
            _handlers.TryAdd(handler, default);
            return handler.GetUpdate();
        }

        public void Consume(Update update)
        {
            foreach (var (handler, _) in _handlers)
            {
                if (handler.HandleUpdate(update))
                {
                    return;
                }
            }

            Updates.Enqueue(update);
        }
    }
}