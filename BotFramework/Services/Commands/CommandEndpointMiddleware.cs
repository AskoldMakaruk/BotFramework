using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using Microsoft.Extensions.Logging;

namespace BotFramework.Middleware;

public class CommandEndpointMiddleware
{
    private readonly UpdateDelegate _next;

    public CommandEndpointMiddleware(UpdateDelegate next)
    {
        _next = next;
    }

    public Task Invoke(UpdateContext                       context,
                       IEnumerable<IEndpoitBuilder>        builders,
                       ILogger<CommandEndpointMiddleware>? logger = null)
    {
        var endpoints = builders.SelectMany(a => a.Get()).ToList();

        logger?.LogTrace("Added {Count} commands:\n{CommandList}", endpoints.Count,
            string.Join(",\n", endpoints.Select(a => a.Name + " " + a.Priority)));

        context.Endpoints.AddRange(endpoints);
        return _next.Invoke(context);
    }
}

//todo as alternetive to IdentityMiddleware for IUserScopeStorage pipeline injection