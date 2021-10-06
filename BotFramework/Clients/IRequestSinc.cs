using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Requests.Abstractions;

namespace BotFramework.Clients
{
    public interface IRequestSinc
    {
        public Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    }
}