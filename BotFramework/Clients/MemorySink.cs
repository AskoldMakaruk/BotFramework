using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Requests.Abstractions;

namespace BotFramework.Clients
{
    public class MemorySink : IRequestSinc
    {
        private readonly ConcurrentQueue<TaskCompletionSource<object>?> GetTasks = new();

        private readonly ConcurrentQueue<object> RequestToSend   = new ConcurrentQueue<object>();
        private readonly ConcurrentQueue<object> TelegramReplies = new ConcurrentQueue<object>();

        public Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            TelegramReplies.TryDequeue(out var reply);

            lock (GetTasks)
            {
                GetTasks.TryPeek(out var task);
                if (task?.Task.IsCompleted == false)
                {
                    task.SetResult(request);
                    GetTasks.TryDequeue(out _);
                    return Task.FromResult((TResponse)reply!);
                }
            }

            RequestToSend.Enqueue(request);

            return Task.FromResult((TResponse)reply!);
        }

        public ValueTask<TResponse> GetRequest<TResponse>(Func<TResponse, bool>? filter = null)
        {
            TResponse? updateToReturn = default;
            while (RequestToSend.TryDequeue(out var update))
            {
                if (update is not TResponse item || filter?.Invoke(item) == false)
                {
                    continue;
                }

                updateToReturn = item;
                break;
            }

            if (updateToReturn is not null)
            {
                return ValueTask.FromResult(updateToReturn);
            }

            var source = new TaskCompletionSource<object>();
            lock (GetTasks)
            {
                GetTasks.Enqueue(source);
            }
            var task = source?.Task.ContinueWith(a =>
                       (TResponse)a.GetAwaiter().GetResult())
                       ?? Task.FromResult(default(TResponse))!;
            return new ValueTask<TResponse>(task);
        }
    }
}