using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Services.Factories;
using Telegram.Bot.Types;

namespace BotFramework.Middleware
{
    public class UpdateMiddleware
    {
        private readonly UpdateDelegate _next;

        public UpdateMiddleware(UpdateDelegate next)
        {
            _next = next;
        }

        public Task Invoke(Update update, UpdateFactory factory)
        {
            factory.CurrentUpdate = update;
            return _next(update);
        }
    }

    // public class AuthorizationMiddleware
    // {
    //     private readonly UpdateDelegate _next;
    //
    //     public AuthorizationMiddleware(UpdateDelegate next) => _next = next;
    //
    //     public Task Invoke(Update update, UpdateContext context)
    //     {
    //         return _next(update);
    //     }
    // }
}