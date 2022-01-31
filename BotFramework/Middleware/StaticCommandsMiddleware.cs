using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace BotFramework.Middleware;

public record StaticCommandsList(IReadOnlyList<Type> Types);

public record ControllersList(IReadOnlyList<Type> Types);

public class PossibleCommands
{
    public List<ICommand> Commands { get; set; } = new();
}

public class StaticCommandsMiddleware
{
    private readonly List<IStaticCommand> commands;
    private readonly UpdateDelegate       _next;

    public StaticCommandsMiddleware(IServiceProvider services, UpdateDelegate next, StaticCommandsList staticCommands)
    {
        _next = next;
        var scope = services.CreateScope();
        commands = staticCommands.Types.Select(scope.ServiceProvider.GetService)
                                 .Cast<IStaticCommand>()
                                 .ToList();
    }

    public Task Invoke(Update update, PossibleCommands possibleCommands)
    {
        possibleCommands.Commands.AddRange(commands);
        return _next.Invoke(update);
    }
}