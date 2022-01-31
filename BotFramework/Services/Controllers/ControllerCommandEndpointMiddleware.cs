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


    private IEnumerable<IStaticCommand> GetCommands(Type controllerType)
    {
        var baseType = typeof(CommandAttribute);
        var methods  = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

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

            var predicate = commandAttributes.Select(a => a.Suitable).Aggregate((func, attribute) => func + attribute);

            yield return new ControllerEndpointCommand(priority, predicate, method) { ControllerType = controllerType };
        }
    }
}