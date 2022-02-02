using System;
using System.Reflection;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Services.Commands.Attributes;

namespace BotFramework.Services.Commands;

[IgnoreReflection]
internal class ControllerEndpointCommand : ICommand
{
    public string Name => ControllerType.Name + "." + _method.Name;

    private readonly CommandPredicate _predicate;
    private readonly MethodBase       _method;
    public readonly  Type             ControllerType;

    public ControllerEndpointCommand(CommandPredicate predicate, MethodBase method, Type controllerType)
    {
        _predicate     = predicate;
        _method        = method;
        ControllerType = controllerType;
    }

    public Task Execute(UpdateContext context)
    {
        return Task.Run(() => _method.Invoke(context.RequestServices.GetService(ControllerType), Array.Empty<object>()));
    }

    public bool? Suitable(UpdateContext context)
    {
        return _predicate.Invoke(context) ?? false;
    }
}