using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;

namespace BotFramework.Middleware;

public class CommandEndpointMiddleware
{
    private readonly UpdateDelegate _next;

    public CommandEndpointMiddleware(UpdateDelegate next)
    {
        _next = next;
    }

    public Task Invoke(UpdateContext context, IEnumerable<IEndpoitBuilder> builders)
    {
        context.Endpoints.AddRange(builders.SelectMany(a => a.Get()));
        return _next.Invoke(context);
    }
}

//todo as alternetive to IdentityMiddleware for IUserScopeStorage pipeline injection