using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;

namespace BotFramework.Clients
{
    public class TelegramSink : IRequestSinc
    {
        private readonly ITelegramBotClient _botClient;

        public TelegramSink(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return _botClient.MakeRequestAsync(request, cancellationToken);
        }
    }
}