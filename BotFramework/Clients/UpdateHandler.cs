using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    public class UpdateHandler
    {
        public Task? CurrentTask;
        public bool  IsDone => CurrentTask.IsCompleted;

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
}