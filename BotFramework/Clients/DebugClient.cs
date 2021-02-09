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
            UserId = (int)update.GetFromsId();
            Consume(update);
            CurrentTask = command.Execute(this);
            Console.WriteLine(UserId);
        }

        public Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            RequestToSend.TryAdd(request);
            TelegramReplies.TryTake(out var reply);
            var task = new TaskCompletionSource<TResponse>();
            task.SetResult((TResponse) (reply));

            return task.Task;
        }

        public ValueTask<IRequest<TResponse>> GetRequest<TResponse>(Func<IRequest<TResponse>, bool>? filter = null)
        {
            IRequest<TResponse>? updateToReturn = null;
            while (RequestToSend.TryTake(out var update))
            {
                if (update is not IRequest<TResponse> item || filter?.Invoke(item) == false)
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

            return new ValueTask<IRequest<TResponse>>(new TaskCompletionSource<IRequest<TResponse>>().Task);
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