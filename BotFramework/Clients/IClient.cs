using System;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.BotTask;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    public interface IClient
    {
        Task<TResponse> MakeRequestAsync<TResponse>(
            IRequest<TResponse> request,
            CancellationToken   cancellationToken = default(CancellationToken));

        BasicBotTask GetUpdateAsync(Func<Update, bool>? filter = null);
        long         UserId { get; }
    }
}