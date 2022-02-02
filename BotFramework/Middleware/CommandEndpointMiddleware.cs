using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Services.Commands;
using BotFramework.Services.Commands.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace BotFramework.Middleware;

public record StaticCommandsList(IReadOnlyList<Type> Types);

public record ControllersList(IReadOnlyList<Type> Types);

//todo move commands to ICommandProvider with different implementations
public class CommandEndpointMiddleware
{
    private readonly List<ICommand>                  commands;
    private readonly List<ControllerEndpointCommand> controllerCommands;
    private readonly UpdateDelegate                  _next;

    public CommandEndpointMiddleware(IServiceProvider services, UpdateDelegate next, StaticCommandsList staticCommands,
                                     ControllersList  controllersList)
    {
        _next = next;
        var scope = services.CreateScope();
        commands = staticCommands.Types
                                 .Select(scope.ServiceProvider.GetService)
                                 .Cast<ICommand>()
                                 .ToList();

        controllerCommands = controllersList.Types.SelectMany(GetControllerCommands)
                                            .ToList();
    }

    public Task Invoke(UpdateContext context)
    {
        context.Endpoints.AddRange(commands.Select(CreateCommandEndpoint));
        context.Endpoints.AddRange(controllerCommands.Select(CreateControllerEndpoint));
        return _next.Invoke(context);
    }

    public Endpoint CreateCommandEndpoint(ICommand command)
    {
        var (commandPredicate, endpointPriority) = GetMemberAttributes(command.GetType());

        commandPredicate ??= DefaultPredicate;
        return new Endpoint
        {
            Name = command.ToString()!,
            Delegate = update =>
            {
                var newCommand = (ICommand)update.RequestServices.GetService(command.GetType())!;
                return newCommand.Execute(update);
            },
            Priority = endpointPriority ?? EndpointPriority.Last,
            CommandPredicate = (UpdateContext context) =>
            (command.Suitable(context) ?? false) && (commandPredicate(context) ?? false),
        };
    }

    private Endpoint CreateControllerEndpoint(ControllerEndpointCommand command)
    {
        var (commandPredicate, endpointPriority) = GetMemberAttributes(command.GetType());

        commandPredicate ??= DefaultPredicate;

        return new Endpoint
        {
            Name     = command.Name,
            Delegate = command.Execute,
            Priority = endpointPriority ?? EndpointPriority.Last,
            CommandPredicate = (UpdateContext context) =>
            (command.Suitable(context) ?? false) && (commandPredicate(context) ?? false),
        };
    }


    private IEnumerable<ControllerEndpointCommand> GetControllerCommands(Type controllerType)
    {
        var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        foreach (var method in methods)
        {
            var (predicate, priority) = GetMemberAttributes(method);
            if (predicate == default && priority == default)
            {
                continue;
            }

            yield return new ControllerEndpointCommand(predicate ?? DefaultPredicate, method, controllerType);
        }
    }

    private (CommandPredicate?, EndpointPriority?) GetMemberAttributes(MemberInfo type)
    {
        var attrs             = type.GetCustomAttributes();
        var commandAttributes = attrs.Where(a => a.GetType().IsSubclassOf(AttributeType)).Cast<CommandAttribute>().ToList();

        if (commandAttributes.Count == 0)
        {
            return default;
        }

        var priority = commandAttributes.FirstOrDefault(a => a.EndpointPriority != null)?.EndpointPriority;

        bool? Predicate(UpdateContext context)
        {
            return commandAttributes.Count == 0 || commandAttributes.Select(a => a.Suitable(context)).All(a => a ?? false);
        }

        return (Predicate, priority);
    }

    private static readonly CommandPredicate DefaultPredicate = _ => true;
    private static readonly Type             AttributeType    = typeof(CommandAttribute);
}