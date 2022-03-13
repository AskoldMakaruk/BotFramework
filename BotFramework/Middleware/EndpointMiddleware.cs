using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Services;
using BotFramework.Services.Clients;
using Microsoft.Extensions.Logging;

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

    public Task Invoke(UpdateContext                     updateContext,
                       ICommandStateMachine              client,
                       ILogger<PriorityCommandExcecutor> logger)
    {
        var id = updateContext.Update.GetId()!.Value;

        _storage.Get(id).GetPriorityUpdateConsumer().Consume(updateContext, client, logger);

        return _next.Invoke(updateContext);
    }
}