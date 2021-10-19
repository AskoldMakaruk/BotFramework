using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Requests.Abstractions;

namespace BotFramework.Abstractions
{
    public interface IRequestSinc
    {
        public Task<TResponse> MakeRequest<TResponse>(
            IRequest<TResponse> request,
            CancellationToken   cancellationToken = default);
    }
}