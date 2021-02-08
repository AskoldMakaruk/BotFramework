using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Commands;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotFramework.Middleware
{
    public record StaticCommandsList(List<Type> StaticCommandsTypes);

    public class StaticCommandsMiddleware
    {
        private readonly IServiceProvider     _services;
        private readonly List<IStaticCommand> commands;
        private readonly UpdateDelegate       _next;


        public StaticCommandsMiddleware(IServiceProvider services, UpdateDelegate next, StaticCommandsList staticCommands)
        {
            _services = services;
            _next     = next;
            var scope = _services.CreateScope();
            commands = staticCommands.StaticCommandsTypes.Select(scope.ServiceProvider.GetService)
                                     .Cast<IStaticCommand>()
                                     .ToList();
        }

        public Task Invoke(Update update, DictionaryContext dictionaryContext)
        {
            var command = commands.FirstOrDefault(t => t.SuitableFirst(update));
            if (command is not null)
            {
                command = (IStaticCommand) _services.GetService(command.GetType())!;
                dictionaryContext.Handlers.AddFirst(new Client(command, _services.GetService<ITelegramBotClient>()!, update));
                return Task.CompletedTask;
            }

            var currentCommand = dictionaryContext.Handlers.FirstOrDefault(t => !t.IsDone);
            if (currentCommand is not null)
            {
                currentCommand.Consume(update);
                return Task.CompletedTask;
            }

            command = commands.FirstOrDefault(t => t.SuitableLast(update));
            if (command is not null)
            {
                command = (IStaticCommand) _services.GetService(command.GetType())!;
                dictionaryContext.Handlers.AddFirst(new Client(command, _services.GetService<ITelegramBotClient>()!, update));
                return Task.CompletedTask;
            }

            return _next(update);
        }
    }

    public static class StaticCommandsEndpoint
    {
        public static void UseStaticCommands(this IAppBuilder builder, StaticCommandsList staticCommands)
        {
            builder.UseMiddleware<StaticCommandsMiddleware>(staticCommands);
        }
    }
}