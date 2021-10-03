using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Helpers;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    public class DebugClient : IClient, IUpdateConsumer
    {
        //out requests 
        //telegram replies to those requests
        //user messages
        private TaskCompletionSource<object>? GetRequestTask;

        //todo make ProducerConsumerTaskCollectionidk
        private readonly IProducerConsumerCollection<object> RequestToSend   = new ConcurrentQueue<object>();
        private readonly IProducerConsumerCollection<object> TelegramReplies = new ConcurrentQueue<object>();
        private readonly IProducerConsumerCollection<Update> UpdatesToHandle = new ConcurrentQueue<Update>();

        private Task                          CurrentTask;
        private TaskCompletionSource<Update>? CurrentBasicBotTask;
        private Func<Update, bool>?           CurrentFilter;
        private Action<Update>?               OnFilterFail;
        public  long                          UserId { get; private set; }

        public void Initialize(ICommand command, Update update)
        {
            UserId = (int) update.GetId()!;
            Consume(update);
            CurrentTask = command.Execute(this);
        }

        public Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            RequestToSend.TryAdd(request);
            TelegramReplies.TryTake(out var reply);
            if (GetRequestTask?.Task.IsCompleted == false)
            {
                GetRequestTask.SetResult(request);
            }

            return Task.FromResult((TResponse) (reply!));
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
            return new ValueTask<TResponse>(GetRequestTask?.Task.ContinueWith(a => (TResponse) a.GetAwaiter().GetResult()) ?? Task.FromResult(default(TResponse)));
        }

        public ValueTask<Update> GetUpdate(Func<Update, bool>? filter = null, Action<Update>? onFilterFail = null)
        {
            CurrentFilter = filter;
            OnFilterFail  = onFilterFail;
            Update? updateToReturn = null;
            while (UpdatesToHandle.TryTake(out var update))
            {
                if (CurrentFilter?.Invoke(update) != false)
                {
                    updateToReturn = update;
                    break;
                }

                onFilterFail?.Invoke(update);
            }

            if (updateToReturn is not null)
            {
                return ValueTask.FromResult(updateToReturn);
            }

            CurrentBasicBotTask = new TaskCompletionSource<Update>();
            return new ValueTask<Update>(CurrentBasicBotTask.Task);
        }

        public bool IsDone             { get; private set; }
        public bool IsWaitingForUpdate { get; private set; }

        public void Consume(Update update)
        {
            UpdatesToHandle.TryAdd(update);
            if (CurrentBasicBotTask?.Task.IsCompleted != false)
            {
                return;
            }

            while (UpdatesToHandle.TryTake(out var u))
            {
                if (CurrentFilter?.Invoke(u) != false)
                {
                    CurrentBasicBotTask.SetResult(u);
                    break;
                }

                OnFilterFail?.Invoke(u);
            }
        }
    }
}