using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Helpers;
using BotFramework.HostServices;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    public class DebugClient : IUpdateConsumer
    {
        public Task FromUser(Update  update)  => _debugDelegateWrapper.App(update);
        public Task FromUser(Message message) => FromUser(new Update { Message = message });

        private TaskCompletionSource<object>? GetRequestTask;

        //todo telegram reply factory
        //todo make ProducerConsumerTaskCollectionidk
        private readonly IProducerConsumerCollection<object> RequestToSend   = new ConcurrentQueue<object>();
        private readonly IProducerConsumerCollection<object> TelegramReplies = new ConcurrentQueue<object>();

        private Func<Update, bool>? CurrentFilter;
        private Action<Update>?     OnFilterFail;
        public  long                UserId { get; private set; }

        private readonly UpdateHandler                                   _updateHandler;
        private readonly AppRunnerServiceExtensions.DebugDelegateWrapper _debugDelegateWrapper;

        public DebugClient(UpdateHandler updateHandler, AppRunnerServiceExtensions.DebugDelegateWrapper debugDelegateWrapper)
        {
            _updateHandler        = updateHandler;
            _debugDelegateWrapper = debugDelegateWrapper;
        }

        public void Initialize(ICommand command, Update update)
        {
            UserId = (int)update.GetId()!;
            Consume(update);
            _updateHandler.CurrentTask = command.Execute(this);
        }

        public Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            RequestToSend.TryAdd(request);
            TelegramReplies.TryTake(out var reply);
            if (GetRequestTask?.Task.IsCompleted == false)
            {
                GetRequestTask.SetResult(request);
            }

            return Task.FromResult((TResponse)(reply!));
        }

        public ValueTask<TResponse> GetRequest<TResponse>(Func<TResponse, bool>? filter = null)
        {
            TResponse? updateToReturn = default;
            while (RequestToSend.TryTake(out var update))
            {
                if (update is not TResponse item || filter?.Invoke(item) == false)
                {
                    continue;
                }

                updateToReturn = item;
                break;
            }

            if (updateToReturn is not null)
            {
                return ValueTask.FromResult(updateToReturn);
            }

            //this is ugly please refactor
            GetRequestTask = new TaskCompletionSource<object>();
            return new ValueTask<TResponse>(GetRequestTask?.Task.ContinueWith(a => (TResponse)a.GetAwaiter().GetResult())
                                            ?? Task.FromResult(default(TResponse))!);
        }

        public ValueTask<Update> GetUpdate(Func<Update, bool>? filter = null, Action<Update>? onFilterFail = null)
        {
            CurrentFilter = filter;
            OnFilterFail  = onFilterFail;

            return _updateHandler.GetUpdate(CurrentFilter, OnFilterFail);
        }

        public bool IsDone             { get; private set; }
        public bool IsWaitingForUpdate { get; private set; }

        public void Consume(Update update)
        {
            _updateHandler.Consume(update, CurrentFilter, OnFilterFail);
        }
    }
}