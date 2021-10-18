using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    public class UpdateQueue
    {
        private readonly IProducerConsumerCollection<Update>            UpdatesToHandle = new ConcurrentQueue<Update>();
        private readonly ConcurrentQueue<TaskCompletionSource<Update>?> GetTasks        = new();

        public void Consume(Update update, Func<Update, bool>? filter = null, Action<Update>? onFilterFail = null)
        {
            lock (GetTasks)
            {
                GetTasks.TryPeek(out var task);
                if (task?.Task.IsCompleted == false)
                {
                    task.SetResult(update);
                    GetTasks.TryDequeue(out _);
                    return;
                }
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

            var task = new TaskCompletionSource<Update>();
            lock (GetTasks)
            {
                GetTasks.Enqueue(task);
            }
            return new ValueTask<Update>(task.Task);
        }
    }
}