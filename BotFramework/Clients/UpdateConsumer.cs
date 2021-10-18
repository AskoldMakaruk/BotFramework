using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    /// <summary>
    /// Consumer as wrapper on Task 'Update'
    /// Should be deleted after done 
    /// </summary>
    public class UpdateHandler
    {
        private readonly Func<Update, bool>?          filter;
        private readonly Action<Update>?              onFilterFail;
        private readonly Action<UpdateHandler>              onDone;
        private readonly TaskCompletionSource<Update> CompletionSource = new ();
        private readonly ConcurrentQueue<Update>         ExistedBeforeUpdates;

        public bool IsDone => CompletionSource.Task.IsCompleted;
        
        private bool IsSuitableUpdate(Update u)
        {
            var suitable = filter?.Invoke(u) is not false;
            if(!suitable)
                onFilterFail?.Invoke(u);
            return suitable;
        }


        public UpdateHandler(Func<Update, bool>? filter, Action<Update>? onFilterFail, IEnumerable<Update> existingUpdates, Action<UpdateHandler> onDone)
        {
            this.filter          = filter;
            this.onFilterFail    = onFilterFail;
            this.onDone          = onDone;
            ExistedBeforeUpdates = new ConcurrentQueue<Update>(existingUpdates);
        }

        public void HandleUpdate(Update update)
        {
            if(IsDone)
                return;
            if (IsSuitableUpdate(update))
            {
                CompletionSource.SetResult(update);
                onDone(this);
            }
        }

        public ValueTask<Update> GetUpdate()
        {
            Update? updateToReturn = null;

            while (ExistedBeforeUpdates.TryDequeue(out var update))
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
                onDone(this);
                return ValueTask.FromResult(updateToReturn);
            }

            return new ValueTask<Update>(CompletionSource.Task);
        }
    }
}