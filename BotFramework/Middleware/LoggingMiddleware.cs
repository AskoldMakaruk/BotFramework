using System.Threading.Tasks;
using BotFramework.Abstractions;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace BotFramework.Middleware;

/// <summary>
/// Middleware that logs all incoming messages from users.
/// </summary>
public class LoggingMiddleware
{
    private readonly UpdateDelegate _next;
    private readonly ILogger?       _logger;

    public LoggingMiddleware(UpdateDelegate next, ILogger<LoggingMiddleware>? logger)
    {
        _next   = next;
        _logger = logger;
    }

    public Task Invoke(UpdateContext update)
    {
        var info = update.Update.GetInfoFromUpdate();

        _logger?.LogInformation("{UpdateType} {MessageType} | {From}: {Contents}",
            info.UpdateType,
            info.MessageType,
            info.From,
            info.Contents);

        return _next.Invoke(update);
    }
}