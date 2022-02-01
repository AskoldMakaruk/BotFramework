using System.Collections.Concurrent;
using System.Linq;
using BotFramework.Abstractions;
using Microsoft.Extensions.Logging;

namespace BotFramework.Services.Clients;

public class PriorityCommandExcecutor
{
    private readonly ConcurrentBag<ICommandStateMachine> consumers = new();

    public void Consume(UpdateContext                     context,
                        ICommandStateMachine              client,
                        ILogger<PriorityCommandExcecutor> logger)
    {
        var commands = context.Endpoints;
        var info     = context.Update.GetInfoFromUpdate();
        
        if (CheckPriority(EndpointPriority.First))
        {
            return;
        }

        if (consumers.FirstOrDefault(a => !a.IsDone) is { } consumer)
        {
            consumer.Consume(context.Update);
            return;
        }

        if (CheckPriority(EndpointPriority.Last))
        {
            return;
        }

        logger.LogDebug("No suitable found to handle {Names} {Update}", info.FromName, info.UpdateType);
        
        bool CheckPriority(EndpointPriority priority)
        {
            if (commands.FirstOrDefault(a => a.Priority == priority && (a.CommandPredicate.Invoke(context) ?? false)) is not
                { } endpoint)
            {
                return false;
            }

            
            logger.LogDebug("{Endpoint} is suitable {Priority} to handle {Names} {Update}", endpoint.Name, priority,
                info.FromName, info.UpdateType);
            client.Initialize(endpoint.Delegate.Invoke(context));
            client.Consume(context.Update);

            consumers.Add(client);
            return true;
        }
    }
}