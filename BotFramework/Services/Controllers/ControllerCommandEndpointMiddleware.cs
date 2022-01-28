using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Middleware;
using BotFramework.Services.Controllers.Attributes;
using Telegram.Bot.Types;

namespace BotFramework.Services.Controllers;

public class ControllerCommandEndpointMiddleware
{
    private readonly List<IStaticCommand> commands;
    private readonly UpdateDelegate       _next;

    public ControllerCommandEndpointMiddleware(IServiceProvider services, ControllersList controllersList, UpdateDelegate next)
    {
        _next    = next;
        commands = controllersList.Types.SelectMany(GetCommands).ToList();
    }

    public Task Invoke(Update update, PossibleCommands possibleCommands)
    {
        possibleCommands.Commands.AddRange(commands);
        return _next.Invoke(update);
    }


    private IEnumerable<IStaticCommand> GetCommands(Type type)
    {
        var baseType = typeof(CommandAttribute);
        var methods  = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        foreach (var method in methods)
        {
            var attrs = method.GetCustomAttributes();

            var commandAttributes =
            attrs.Where(a => a.GetType().IsSubclassOf(baseType)).Cast<CommandAttribute>().ToList();
            if (commandAttributes.Count == 0)
            {
                continue;
            }

            var priority = commandAttributes.FirstOrDefault(a => a.EndpointPriority != null)?.EndpointPriority
                           ?? EndpointPriority.Last;

            var predicate = commandAttributes.Aggregate((UpdatePredicate)Seed, (func, attribute) => func + attribute.Suitable);

            bool? Seed(Update context) => true;

            yield return new ControllerEndpointCommand(priority, predicate, method) { ControllerType = type };
        }
    }
}