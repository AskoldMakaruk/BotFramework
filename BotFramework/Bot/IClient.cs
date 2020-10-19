using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.BotTask;
using BotFramework.Commands;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using File = Telegram.Bot.Types.File;

namespace BotFramework.Bot
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