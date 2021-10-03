using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Helpers;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    public class UpdateHandler
    {
        public Task? CurrentTask;
        public bool  IsDone             => CurrentTask.IsCompleted;
        public bool  IsWaitingForUpdate => CurrentBasicBotTask?.Task.IsCompleted == false;

        private readonly IProducerConsumerCollection<Update> UpdatesToHandle = new ConcurrentQueue<Update>();
        private          TaskCompletionSource<Update>?       CurrentBasicBotTask;

        public ValueTask<Update> GetUpdate(Func<Update, bool>? filter = null, Action<Update>? onFilterFail = null)
        {
            Update? updateToReturn = null;

            while (UpdatesToHandle.TryTake(out var update))
            {
                if (filter?.Invoke(update) != false)
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

        public void Consume(Update update, Func<Update, bool>? filter = null, Action<Update>? onFilterFail = null)
        {
            UpdatesToHandle.TryAdd(update);
            if (CurrentBasicBotTask?.Task.IsCompleted != false)
            {
                return;
            }

            while (UpdatesToHandle.TryTake(out var u))
            {
                if (filter?.Invoke(u) != false)
                {
                    CurrentBasicBotTask.SetResult(u);
                    break;
                }

                onFilterFail?.Invoke(u);
            }
        }
    }

    /// <inheritdoc cref="IClient" />
    /// >
    public class Client : IClient, IUpdateConsumer
    {
        private readonly UpdateHandler      _updateHandler;
        private readonly ITelegramBotClient _client;

        private Func<Update, bool>? CurrentFilter;
        private Action<Update>?     OnFilterFail;

        public long UserId             { get; private set; }
        public bool IsDone             => _updateHandler.IsDone;
        public bool IsWaitingForUpdate => _updateHandler.IsWaitingForUpdate;

        public Client(ITelegramBotClient client, UpdateHandler updateHandler)
        {
            _client        = client;
            _updateHandler = updateHandler;
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

        public Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return _client.MakeRequestAsync(request, cancellationToken);
        }

        public void Consume(Update update)
        {
            _updateHandler.Consume(update, CurrentFilter, OnFilterFail);
        }
    }
}