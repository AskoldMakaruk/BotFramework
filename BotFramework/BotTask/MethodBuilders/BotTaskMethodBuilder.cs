using System;
using System.Runtime.CompilerServices;
using BotFramework.BotTask.Awaiters;

namespace BotFramework.BotTask
{
    public struct BotTaskMethodBuilder<TResult>
    {
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
            Task.IsCompleted = true;
            Task.Result      = result;
            Task.OnCompleted?.Invoke();
        }

        public void SetException(Exception exception)
        {
            Task.Exception   = exception;
            Task.IsCompleted = true;
        }

        public BotTask<TResult> Task { get; }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter      awaiter,
            ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
        {

            if (typeof(TAwaiter) != typeof(BotAwaiter<>))
            {
                Task.NonBotTask = awaiter;
            }
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter      awaiter,
            ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
        {
            if (typeof(TAwaiter) != typeof(BotAwaiter<>))
            {
                Task.NonBotTask_ = awaiter;
                Task.NonBotTask = awaiter;
            }
            awaiter.OnCompleted(stateMachine.MoveNext);
        }
    }
}