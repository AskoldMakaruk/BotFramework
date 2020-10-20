using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotFramework.BotTask
{
    [AsyncMethodBuilder(typeof(BotTaskMethodBuilder<>))]
    public class BotTask<T>
    {
        internal BotTask() { }

        public  bool       IsCompleted { get; private set; }
        private T          result;
        private Exception? exception;

        public T Result
        {
            get => result;
            internal set
            {
                IsCompleted = true;
                result      = value;
                InvokeOnCompleted();
            }
        }

        public Exception? Exception
        {
            get => exception;
            internal set
            {
                IsCompleted = true;
                exception   = value;
                InvokeOnCompleted();
            }
        }

        private void InvokeOnCompleted()
        {
            foreach (var action in onCompletedActions)
               action.Invoke();
        }
        private readonly List<Action> onCompletedActions = new List<Action>();
        public           Action       OnCompleted { set => onCompletedActions.Add(value); }

        public BotAwaiter<T> GetAwaiter()
        {
            return new BotAwaiter<T>(this);
        }
    }

    [AsyncMethodBuilder(typeof(BotTaskMethodBuilder<>))]
    public class BasicBotTask : BotTask<Update>
    {
        private readonly Func<Update, bool>? filter;

        public BasicBotTask(Func<Update, bool>? filter)
        {
            this.filter = filter;
        }

        public void HandleUpdate(Update update)
        {
            if (IsCompleted) return;
            if (filter == null || filter.Invoke(update))
                Result      = update;
        }
    }
}