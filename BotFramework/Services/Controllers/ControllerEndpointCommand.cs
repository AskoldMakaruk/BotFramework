using System;
using System.Reflection;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Services.Controllers;

internal class ControllerEndpointCommand : IStaticCommand
{
    private readonly EndpointPriority _priority;
    private readonly UpdatePredicate  _predicate;
    private readonly MethodBase       _method;

    public object ControllerIntance { get; set; }
    public Type   ControllerType    { get; set; }

    public ControllerEndpointCommand(EndpointPriority priority, UpdatePredicate predicate, MethodBase method)
    {
        _priority  = priority;
        _predicate = predicate;
        _method    = method;
    }

    public Task Execute(IClient client)
    {
        return Task.Run(() => _method.Invoke(ControllerIntance, Array.Empty<object>()));
    }

    public bool SuitableLast(Update update)
    {
        return _priority == EndpointPriority.Last && (_predicate.Invoke(update) ?? false);
    }

    public bool SuitableFirst(Update update)
    {
        return _priority == EndpointPriority.First && (_predicate.Invoke(update) ?? false);
    }
}

public delegate bool? UpdatePredicate(Update update);