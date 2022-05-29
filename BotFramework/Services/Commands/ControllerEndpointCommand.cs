using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Services.Commands.Attributes;

namespace BotFramework.Services.Commands;

[IgnoreReflection]
internal class ControllerEndpointCommand : ICommand
{
    public string Name => ControllerType.Name + "." + _method.Name;

    private readonly CommandPredicate                    _predicate;
    private readonly MethodBase                          _method;
    public readonly  Type                                ControllerType;
    public readonly  IReadOnlyList<CommandAttributeBase> Attributes;
    public           EndpointPriority                    Priority { get; init; }

    public ControllerEndpointCommand(
        CommandPredicate                    predicate,
        MethodBase                          method,
        Type                                controllerType,
        IReadOnlyList<CommandAttributeBase> attributes)
    {
        _predicate     = predicate;
        _method        = method;
        ControllerType = controllerType;
        Attributes     = attributes;
    }

    public Task Execute(UpdateContext context)
    {
        return Task.Run(async () =>
        {
            try
            {
                await (Task)_method.Invoke(context.RequestServices.GetService(ControllerType), Array.Empty<object>())!;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return Task.CompletedTask;
        });
    }

    public bool? Suitable(UpdateContext context)
    {
        return _predicate.Invoke(context) ?? false;
    }
}