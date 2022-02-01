using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Requests.Abstractions;

namespace BotFramework.Abstractions;

public interface IRequestSinc
{
    /// <summary>
    /// Sends a request.
    /// </summary>
    /// <param name="request">Request to be sent.</param>
    /// <param name="cancellationToken">A cancelation token.</param>
    /// <typeparam name="TResponse">Type of response expected from server.</typeparam>
    /// <returns>Task with response from server.</returns>
    public Task<TResponse> MakeRequest<TResponse>(
        IRequest<TResponse> request,
        CancellationToken   cancellationToken = default);
}