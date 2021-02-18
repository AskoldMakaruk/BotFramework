using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace BotFramework.Middleware
{
    public record StaticCommandsList(List<Type> StaticCommandsTypes);

    public class StaticCommandsMiddleware
    {
        private readonly IServiceProvider                             _services;
        private readonly List<IStaticCommand>                         commands;
        private readonly UpdateDelegate                               _next;
        private readonly ConcurrentDictionary<long, IServiceProvider> _providers = new();



        public StaticCommandsMiddleware(IServiceProvider services, UpdateDelegate next, StaticCommandsList staticCommands)
        {
            _services = services;
            _next     = next;
            var scope = _services.CreateScope();
            commands = staticCommands.StaticCommandsTypes.Select(scope.ServiceProvider.GetService)
                                     .Cast<IStaticCommand>()
                                     .ToList();
        }

        public Task Invoke(Update update, DictionaryContext dictionaryContext, WrappedServiceProvider provider)
        {
            if (_services.GetService<IUpdateConsumer>() is not { } client) //checking for null 
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
                provider.Provider = _providers.GetOrAdd(update.GetUser()!.Id, provider.Provider);
                return true;
            }

            return _next(update);
        }
    }

    public static class StaticCommandsEndpoint
    {
        public static void UseStaticCommands(this IAppBuilder builder, StaticCommandsList staticCommands)
        {
            builder.Services.AddSingleton(staticCommands);
            builder.UseMiddleware<StaticCommandsMiddleware>(staticCommands);
            foreach (var command in staticCommands.StaticCommandsTypes)
                builder.Services.AddScoped(command);
        }

        public static void UseStaticCommands(this IAppBuilder builder)
        {
            var staticCommands = AppDomain.CurrentDomain.GetAssemblies()
                                          .SelectMany(s => s.GetTypes())
                                          .Where(p => typeof(ICommand).IsAssignableFrom(p) && !p.IsAbstract)
                                          .ToList();
            foreach (var command in staticCommands)
                builder.Services.AddScoped(command);
            builder.Services.AddSingleton(new StaticCommandsList(staticCommands));
            builder.UseMiddleware<StaticCommandsMiddleware>(new StaticCommandsList(staticCommands));
        }
    }
}