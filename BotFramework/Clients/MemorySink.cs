using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Requests.Abstractions;

namespace BotFramework.Clients
{
    public class MemorySink : IRequestSinc
    {
        private TaskCompletionSource<object>? GetRequestTask;

        private readonly IProducerConsumerCollection<object> RequestToSend   = new ConcurrentQueue<object>();
        private readonly IProducerConsumerCollection<object> TelegramReplies = new ConcurrentQueue<object>();

        public Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            RequestToSend.TryAdd(request);
            TelegramReplies.TryTake(out var reply);
            if (GetRequestTask?.Task.IsCompleted == false)
            {
                GetRequestTask.SetResult(request);
            }

            return Task.FromResult((TResponse)(reply!));
        }

        public ValueTask<TResponse> GetRequest<TResponse>(Func<TResponse, bool>? filter = null)
        {
            TResponse? updateToReturn = default;
            while (RequestToSend.TryTake(out var update))
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

            GetRequestTask = new TaskCompletionSource<object>();
            
            var task = GetRequestTask?.Task.ContinueWith(a =>
                       (TResponse)a.GetAwaiter().GetResult())
                       ?? Task.FromResult(default(TResponse))!;
            return new ValueTask<TResponse>(task);
        }
    }
}