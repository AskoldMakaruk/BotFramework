using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Responses;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    public class Client : IClient
    {
        private TelegramBotClient                   _client;
        public  TaskCompletionSource<Update>?       CurrentBasicBotTask;
        public  Task<Response>?                     CurrentTask;
        private Func<Update, bool>?                 CurrentFilter;
        private Action<Update>?                     OnFilterFail;
        private IProducerConsumerCollection<Update> UpdatesToHandle = new ConcurrentQueue<Update>();
        public Client(TelegramBotClient client, long userId) => (_client, UserId) = (client, userId);

        public ValueTask<Update> GetUpdate(Func<Update, bool>? filter = null, Action<Update>? onFilterFail = null)
        {
            CurrentFilter = filter;
            OnFilterFail = onFilterFail;
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
                return ValueTask.FromResult(updateToReturn);
            CurrentBasicBotTask = new TaskCompletionSource<Update>();
            return new ValueTask<Update>(CurrentBasicBotTask.Task);
        }

        public void HandleUpdate(Update update)
        {
            UpdatesToHandle.TryAdd(update);
            if (CurrentBasicBotTask?.Task.IsCompleted != false)
                return;
            while (UpdatesToHandle.TryTake(out update))
            {
                if (CurrentFilter?.Invoke(update) != false)
                {
                    CurrentBasicBotTask.SetResult(update);
                    break;
                }
                OnFilterFail?.Invoke(update);
            }
        }

        public Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request,
                                                      CancellationToken   cancellationToken = default(CancellationToken)) =>
        _client.MakeRequestAsync(request, cancellationToken);

        public long UserId { get; }
    }
}