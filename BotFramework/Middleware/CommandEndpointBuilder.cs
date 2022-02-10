using System;
using System.Collections.Generic;
using System.Linq;
using BotFramework.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BotFramework.Middleware;

public class CommandEndpointBuilder : IEndpoitBuilder
{
    private readonly List<ICommand> commands;


    public CommandEndpointBuilder(IServiceProvider services, StaticCommandsList staticCommands)
    {
        var scope = services.CreateScope();
        commands = staticCommands.Types
                                 .Select(scope.ServiceProvider.GetService)
                                 .Cast<ICommand>()
                                 .ToList();
    }

    public Endpoint Get(ICommand command)
    {
        var endpoint = CreateEndpoint(command);
        endpoint.Name = command.ToString()!;
        endpoint.Delegate = update =>
        {
            var newCommand = (ICommand)update.RequestServices.GetService(command.GetType())!;
            return newCommand.Execute(update);
        };

        return endpoint;
    }

    public IEnumerable<Endpoint> Get()
    {
        return commands.Select(CreateCommandEndpoint);
    }


    public Endpoint CreateCommandEndpoint(ICommand command)
    {
        var endpoint = CreateEndpoint(command);
        endpoint.Name = command.ToString()!;
        endpoint.Delegate = update =>
        {
            var newCommand = (ICommand)update.RequestServices.GetService(command.GetType())!;
            return newCommand.Execute(update);
        };

        return endpoint;
    }


    private Endpoint CreateEndpoint(ICommand command)
    {
        var attrs     = this.GetCommandAttributes(command.GetType()).ToList();
        var predicate = this.GetPredicate(attrs);
        var priority  = this.GetPriority(attrs);
        var state     = this.GetState(attrs);

        return new Endpoint
        {
            Priority     = priority ?? EndpointPriority.Last,
            CommandState = state,
            CommandPredicate = (UpdateContext context) =>
            (command.Suitable(context) ?? false) && (predicate(context) ?? false),
            Attributes = attrs
        };
    }
}