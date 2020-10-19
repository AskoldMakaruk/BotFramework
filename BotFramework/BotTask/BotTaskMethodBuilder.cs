using System;
using System.Runtime.CompilerServices;

namespace BotFramework.BotTask
{
    public readonly struct BotTaskMethodBuilder<TResult>
    {
        public BotTaskMethodBuilder(BotTask<TResult> t)
        {
            Task = t;
        }
        public static BotTaskMethodBuilder<TResult> Create()
        {
           return new BotTaskMethodBuilder<TResult>(new BotTask<TResult>());
        }
        public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            stateMachine.SetStateMachine(stateMachine); //хз
        }

        public void SetResult(TResult result)
        {
            Task.Result      = result;
        }

        public void SetException(Exception exception)
        {
            Task.Exception   = exception;
        }

        public BotTask<TResult> Task { get; }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter      awaiter,
            ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
        {

            if (typeof(TAwaiter).GetGenericTypeDefinition() != typeof(BotAwaiter<>))
            {
                var task = Task;
                Task.IsRunningNonBotTask = true;
                var machine = stateMachine;
                awaiter.OnCompleted(() =>
                {
                    machine.MoveNext();
                    task.IsRunningNonBotTask = false;

                });
                return;
            }
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter      awaiter,
            ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
        {
            if (typeof(TAwaiter).GetGenericTypeDefinition() != typeof(BotAwaiter<>))
            {
                var task = Task;
                Task.IsRunningNonBotTask = true;
                var machine = stateMachine;
                awaiter.OnCompleted(() =>
                {
                    machine.MoveNext();
                    task.IsRunningNonBotTask = false;

                });
                return;
            }
            awaiter.OnCompleted(stateMachine.MoveNext);
        }
    }
}