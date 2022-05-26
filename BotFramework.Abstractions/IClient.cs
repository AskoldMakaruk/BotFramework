using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Abstractions;

/// <summary>
/// This interface only needed to provide an easy way for commands to use <see cref="IRequestSinc"/> and <see cref="IUpdateQueue"/>.
/// </summary>
public interface IClient
{
    
    /// <summary>
    /// Last update that was processed.
    /// </summary>
    public Update Update { get; }
    
    /// <summary>
    ///     Identifier of user. Each user has his/her own <see cref="IClient" />
    /// </summary>
    long UserId { get; }


    /// <inheritdoc cref="IRequestSinc.MakeRequest{TResponse}"/>
    public Task<TResponse> MakeRequest<TResponse>(
        IRequest<TResponse> request,
        CancellationToken   cancellationToken = default);

    /// <inheritdoc cref="IUpdateQueue.GetUpdate"/>
    ValueTask<Update> GetUpdate(Func<Update, bool>? filter = null, Action<Update>? onFilterFail = null);
}