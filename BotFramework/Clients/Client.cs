using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Commands;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    /// <inheritdoc cref="IClient"/>>
    public class Client<T> : IClient, IUpdateConsumer where T: IBotContext
    {
        private ITelegramBotClient                   _client;
        private  TaskCompletionSource<Update>?       CurrentBasicBotTask;
        private  readonly Task                     CurrentTask;
        private Func<Update, bool>?                 CurrentFilter;
        private Action<Update>?                     OnFilterFail;
        private IProducerConsumerCollection<Update> UpdatesToHandle = new ConcurrentQueue<Update>();

        public Client(ICommand<T> command, T context, ITelegramBotClient client)
        {
          _client           = client;
          UserId            = context.ChatId.Id;
          HandleUpdate(context.CurrentUpdate);
          CurrentTask       = command.Execute(this, context);
        } 

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

        public Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request,
                                                      CancellationToken   cancellationToken = default(CancellationToken)) =>
        _client.MakeRequestAsync(request, cancellationToken);

        public long UserId                 { get; }
        public bool IsDone                 => CurrentTask.IsCompleted;
        public bool IsWaitingForUpdate     => CurrentBasicBotTask?.Task.IsCompleted == false;
        public void Consume(Update update) => HandleUpdate(update);
    }
}