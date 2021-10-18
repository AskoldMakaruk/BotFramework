using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    public class UpdateQueue
    {
        private readonly IProducerConsumerCollection<Update> UpdatesToHandle = new ConcurrentQueue<Update>();
        private          TaskCompletionSource<Update>?       CurrentTask;

        public void Consume(Update update, Func<Update, bool>? filter = null, Action<Update>? onFilterFail = null)
        {
            if (CurrentTask?.Task.IsCompleted == false)
            {
                CurrentTask.SetResult(update);
                return;
            }

            UpdatesToHandle.TryAdd(update);
        }

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

            CurrentTask = new TaskCompletionSource<Update>();


            return new ValueTask<Update>(CurrentTask.Task);
        }
    }
}