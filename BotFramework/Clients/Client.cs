using System;
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
        private TelegramBotClient  _client;
        public  BasicBotTask?      CurrentBasicBotTask;
        public  BotTask<Response>? CurrentTask;
        public  Queue<Update>      UpdatesToHandle = new Queue<Update>();
        public  Client(TelegramBotClient client, long userId) => (_client, UserId) = (client, userId);

        public BasicBotTask GetUpdateAsync(Func<Update, bool>? filter = null)
        {
            CurrentBasicBotTask = new BasicBotTask(filter);
            if (UpdatesToHandle.Count > 0)
            {
                CurrentBasicBotTask.HandleUpdate(UpdatesToHandle.Dequeue());
            }

            return CurrentBasicBotTask;
        }

        public void HandleUpdate(Update update)
        {
            if (CurrentTask?.IsRunningNonBotTask == true)
            {
                UpdatesToHandle.Enqueue(update);
                return;
            }

            UpdatesToHandle.Enqueue(update);
            CurrentBasicBotTask?.HandleUpdate(UpdatesToHandle.Dequeue());
        }

        public Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request,
                                                           CancellationToken   cancellationToken = default(CancellationToken)) =>
        _client.MakeRequestAsync(request, cancellationToken);

        public long UserId { get; }
    }
}