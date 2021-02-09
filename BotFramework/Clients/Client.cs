using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    /// <inheritdoc cref="IClient" />
    /// >
    public class Client : IClient, IUpdateConsumer
    {
        private readonly ITelegramBotClient                  _client;
        private          Task?                               CurrentTask;
        private readonly IProducerConsumerCollection<Update> UpdatesToHandle = new ConcurrentQueue<Update>();
        private          TaskCompletionSource<Update>?       CurrentBasicBotTask;
        private          Func<Update, bool>?                 CurrentFilter;
        private          Action<Update>?                     OnFilterFail;

        public Client(ITelegramBotClient client)
        {
            _client = client;
        }

        public void Initialize(ICommand command, Update update)
        {
            UserId = update.Message.Chat.Id;
            HandleUpdate(update);
            CurrentTask = command.Execute(this);
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

        public Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request,
                                                      CancellationToken   cancellationToken = default)
        {
            return _client.MakeRequestAsync(request, cancellationToken);
        }

        public long UserId             { get; private set; }
        public bool IsDone             => CurrentTask.IsCompleted;
        public bool IsWaitingForUpdate => CurrentBasicBotTask?.Task.IsCompleted == false;

        public void Consume(Update update) => HandleUpdate(update);


        public void HandleUpdate(Update update)
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