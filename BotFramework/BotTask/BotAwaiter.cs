using System;
using System.Runtime.CompilerServices;

namespace BotFramework.BotTask
{
    public readonly struct BotAwaiter<T> : ICriticalNotifyCompletion
    {
        public BotAwaiter(BotTask<T> task)
        {
            this.task = task;
        }

        public           bool       IsCompleted => task.IsCompleted;
        private readonly BotTask<T> task;

        public T GetResult()
        {
            return task.Result;
        }

        public void OnCompleted(Action continuation)
        {
            task.OnCompleted = continuation;
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            task.OnCompleted = continuation;
        }
    }
}