using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BotFramework.Abstractions;

namespace BotFramework.Middleware;

public static class IEndpoitBuilderExtensions
{
    internal static CommandPredicate GetPredicate(this IEndpoitBuilder _, IEnumerable<CommandAttribute> attributes)
    {
        bool? Predicate(UpdateContext context)
        {
            return attributes.Select(a => a.Suitable(context)).All(a => a ?? false);
        }

        return Predicate;
    }

    internal static EndpointPriority? GetPriority(this IEndpoitBuilder _, IEnumerable<CommandAttribute> attributes)
    {
        return attributes.FirstOrDefault(a => a.EndpointPriority != null)?.EndpointPriority;
    }

    internal static int? GetState(this IEndpoitBuilder _, IEnumerable<CommandAttribute> attributes)
    {
        var a = attributes.OfType<PersistentStateAttribute>().FirstOrDefault();
        return a?.State;
    }

    internal static IEnumerable<CommandAttribute> GetCommandAttributes(this IEndpoitBuilder _, MemberInfo type)
    {
        var attrs = type.GetCustomAttributes();
        return attrs.Where(a => a.GetType().IsSubclassOf(AttributeType)).Cast<CommandAttribute>().ToList();
    }


    internal static Endpoint CreateEndpoint(this IEndpoitBuilder a, ICommand command)
    {
        var attrs     = a.GetCommandAttributes(command.GetType()).ToList();
        var predicate = a.GetPredicate(attrs);
        var priority  = a.GetPriority(attrs);
        var state     = a.GetState(attrs);

        return new Endpoint
        {
            Priority     = priority ?? EndpointPriority.Last,
            CommandState = state,
            CommandPredicate = (UpdateContext context) =>
            (command.Suitable(context) ?? false) && (predicate(context) ?? false),
            Attributes = attrs
        };
    }

    internal static readonly Type AttributeType = typeof(CommandAttribute);
}