using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.BotTask;
using BotFramework.Commands;
using BotFramework.Responses;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Bot
{
    public class PerUserClient : IClient
    {
        private TelegramBotClient  _client;
        public  BasicBotTask?      CurrentBasicBotTask;
        public  BotTask<Response>? CurrentTask;
        public  Queue<Update>      UpdatesToHandle;
        public PerUserClient(TelegramBotClient client, long userId) => (_client, UserId) = (client, userId);

        public BasicBotTask GetUpdateAsync(Func<Update, bool>? filter = null)
        {
            //бля треба б щось типу валуе таск
            CurrentBasicBotTask ??= new BasicBotTask(filter);
            if (UpdatesToHandle.Count > 0)
            {
                CurrentBasicBotTask.HandleUpdate(UpdatesToHandle.Dequeue());
            }

            return CurrentBasicBotTask;
        }

        public void HandleUpdate(Update update)
        {
            if (CurrentTask != null && !CurrentTask.IsCompleted)
            {
                UpdatesToHandle.Enqueue(update);
                return;
            }
            CurrentBasicBotTask?.HandleUpdate(update);
        }

        public Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request,
                                                           CancellationToken   cancellationToken = default(CancellationToken)) =>
        _client.MakeRequestAsync(request, cancellationToken);

        public long UserId { get; }
    }
}