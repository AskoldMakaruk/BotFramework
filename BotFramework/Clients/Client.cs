using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.BotTask;
using BotFramework.Responses;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    public class Client : IClient
    {
        private TelegramBotClient                   _client;
        public  BasicBotTask?                       CurrentBasicBotTask;
        public  BotTask<Response>?                  CurrentTask;
        public  IProducerConsumerCollection<Update> UpdatesToHandle = new ConcurrentQueue<Update>();
        public Client(TelegramBotClient client, long userId) => (_client, UserId) = (client, userId);

        public BasicBotTask GetUpdateAsync(Func<Update, bool>? filter = null)
        {
            CurrentBasicBotTask = new BasicBotTask(filter);
            while (!CurrentBasicBotTask.IsCompleted && UpdatesToHandle.TryTake(out var update))
                CurrentBasicBotTask.HandleUpdate(update);
            return CurrentBasicBotTask;
        }

        public void HandleUpdate(Update update)
        {
            UpdatesToHandle.TryAdd(update);
            if (CurrentBasicBotTask == null || CurrentBasicBotTask.IsCompleted)
                return;
            while (!CurrentBasicBotTask.IsCompleted && UpdatesToHandle.TryTake(out update))
                CurrentBasicBotTask.HandleUpdate(update);
        }

        public Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request,
                                                           CancellationToken   cancellationToken = default(CancellationToken)) =>
        _client.MakeRequestAsync(request, cancellationToken);

        public long UserId { get; }
    }
}