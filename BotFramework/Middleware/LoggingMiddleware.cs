using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Helpers;
using Serilog;
using Serilog.Context;
using Telegram.Bot.Types;

namespace BotFramework.Middleware
{
    public class LoggingMiddleware
    {
        private readonly UpdateDelegate _next;
        private readonly ILogger        _logger;

        public LoggingMiddleware(UpdateDelegate next, ILogger logger)
        {
            _next   = next;
            _logger = logger;
        }

        public Task Invoke(Update update)
        {
            var info = update.GetInfoFromUpdate();

            using (LogContext.PushProperty("UpdateType", info.UpdateType))
            using (LogContext.PushProperty("MessageType", info.MessageType))
            using (LogContext.PushProperty("From", info.FromName))
            using (LogContext.PushProperty("Contents", info.Contents))
            {
                _logger.Information("{UpdateType} {MessageType} | {From}: {Contents}");
            }

            return _next.Invoke(update);
        }
    }
}