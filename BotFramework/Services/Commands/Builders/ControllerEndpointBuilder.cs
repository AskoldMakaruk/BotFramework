using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BotFramework.Abstractions;
using BotFramework.Services.Commands;

namespace BotFramework.Middleware;

public class ControllerEndpointBuilder : IEndpoitBuilder
{
    private readonly List<ControllerEndpointCommand> controllerCommands;


    public ControllerEndpointBuilder(ControllersList staticCommands)
    {
        controllerCommands = staticCommands.Types.SelectMany(GetControllerCommands)
                                           .ToList();
    }

    public IEnumerable<Endpoint> Get()
    {
        return controllerCommands.Select(CreateControllerEndpoint);
    }

    private Endpoint CreateControllerEndpoint(ControllerEndpointCommand command)
    {
        var endpoint = this.CreateEndpoint(command);
        endpoint.Name     = command.Name;
        endpoint.Delegate = command.Execute;
        endpoint.Attributes.AddRange(command.Attributes);
        endpoint.Priority = command.Priority;

        return endpoint;
    }


    private IEnumerable<ControllerEndpointCommand> GetControllerCommands(Type controllerType)
    {
        var methods              = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        var controllerAttributes = this.GetCommandAttributes(controllerType).ToList();

        foreach (var method in methods)
        {
            var attrs = this.GetCommandAttributes(method).ToList();
            if (attrs.Count == 0)
            {
                continue;
            }

            attrs.AddRange(controllerAttributes);

            var predicate = this.GetPredicate(attrs);

            yield return new ControllerEndpointCommand(predicate, method, controllerType, attrs)
            {
                Priority = this.GetPriority(attrs) ?? EndpointPriority.Last
            };
        }
    }
}