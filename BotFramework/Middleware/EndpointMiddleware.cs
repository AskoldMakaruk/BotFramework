using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Services;
using Telegram.Bot.Types;

namespace BotFramework.Middleware;

public class EndpointMiddleware
{
    private readonly UpdateDelegate   _next;
    private readonly UserScopeStorage _storage;


    public EndpointMiddleware(UpdateDelegate   next,
                              UserScopeStorage storage)
    {
        _next    = next;
        _storage = storage;
    }

    public Task Invoke(Update                 update,
                       UpdateContext          updateContext,
                       ICommandStateMachine client)
    {
        var id = update.GetId()!.Value;

        _storage.Get(id).GetPriorityUpdateConsumer().Consume(updateContext, client);

        return _next.Invoke(update);
    }
}