using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BotFramework.BotTask.Awaiters;
using Telegram.Bot.Types;

namespace BotFramework.BotTask
{
    [AsyncMethodBuilder(typeof(BotTaskMethodBuilder<>))]
    public class BotTask<T>
    {
        internal BotTask() { }

        internal bool       IsCompleted;
        public   Exception? Exception   { get; internal set; }
        public   Action     OnCompleted { get; internal set; }
        internal bool       IsRunningNonBotTask;

        public new BotAwaiter<T> GetAwaiter()
        {
            return new BotAwaiter<T>(this);
        }

        internal T Result;
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
            {
                IsCompleted = true;
                Result      = update;
                OnCompleted?.Invoke();
            }
        }
    }
}