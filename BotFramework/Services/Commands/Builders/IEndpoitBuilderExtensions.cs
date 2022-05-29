using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BotFramework.Abstractions;
using BotFramework.Localization;

namespace BotFramework.Middleware;

public static class IEndpoitBuilderExtensions
{
    internal static CommandPredicate GetPredicate(this IEndpoitBuilder _, IEnumerable<CommandAttributeBase> attributes)
    {
        bool? Predicate(UpdateContext context)
        {
            return attributes.Select(a => a.Suitable(context)).Where(a => a is not null).Cast<bool>().All(a => a);
        }

        return Predicate;
    }

    internal static EndpointPriority? GetPriority(this IEndpoitBuilder _, IEnumerable<CommandAttributeBase> attributes)
    {
        return attributes.FirstOrDefault(a => a.Priority != null)?.Priority;
    }

    internal static int? GetState(this IEndpoitBuilder _, IEnumerable<CommandAttributeBase> attributes)
    {
        var a = attributes.OfType<PersistentStateAttribute>().FirstOrDefault();
        return a?.State;
    }

    internal static IEnumerable<CommandAttributeBase> GetCommandAttributes(this IEndpoitBuilder _, MemberInfo type)
    {
        var attrs = type.GetCustomAttributes();
        return attrs.Where(a => a.GetType().IsSubclassOf(AttributeType)).Cast<CommandAttributeBase>().ToList();
    }


    internal static Endpoint CreateEndpoint(this IEndpoitBuilder a, ICommand command)
    {
        var attrs     = a.GetCommandAttributes(command.GetType()).ToList();
        var predicate = a.GetPredicate(attrs);
        var priority  = a.GetPriority(attrs);
        var state     = a.GetState(attrs);

        return new Endpoint
        {
            Priority         = priority ?? EndpointPriority.Last,
            CommandState     = state,
            CommandPredicate = context => (command.Suitable(context) ?? false) && (predicate(context) ?? false),
            Attributes       = attrs
        };
    }

    internal static readonly Type AttributeType = typeof(CommandAttributeBase);
}