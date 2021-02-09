using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using Microsoft.Extensions.DependencyInjection;
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
            if (_services.GetService<IUpdateConsumer>() is not { } client)
            {
                throw new Exception("Client not found");
            }

            if (Initialize(commands.FirstOrDefault(t => t.SuitableFirst(update))))
            {
                return Task.CompletedTask;
            }

            var currentCommand = dictionaryContext.Handlers.FirstOrDefault(t => !t.IsDone);
            if (currentCommand is not null)
            {
                currentCommand.Consume(update);
                return Task.CompletedTask;
            }

            if (Initialize(commands.FirstOrDefault(t => t.SuitableLast(update))))
            {
                return Task.CompletedTask;
            }

            bool Initialize(ICommand? command)
            {
                if (command is null)
                {
                    return false;
                }

                command = (IStaticCommand) _services.GetService(command.GetType())!;
                client.Initialize(command, update);

                dictionaryContext.Handlers.AddFirst(client);
                return true;
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

        public static void UseStaticCommands(this IAppBuilder builder)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                                    .FirstOrDefault(a => a.GetTypes().Any(t => t.Name == "Program"));
            if (assembly == null)
            {
                throw new Exception("AAAAAAA WHAT THE FUCK!!!");
            }
            var staticCommands = assembly.GetTypes()
                                         .Where(t => t.GetInterfaces().Contains(typeof(ICommand)) && !t.IsAbstract)
                                         .ToList();

            builder.UseMiddleware<StaticCommandsMiddleware>(new StaticCommandsList(staticCommands));
        }
    }
}