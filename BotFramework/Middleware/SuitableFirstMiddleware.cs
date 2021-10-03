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
    public record Consumers(LinkedList<IUpdateConsumer> List);

    public class StaticCommandsEndpoint
    {
        private readonly UpdateDelegate _next;

        public StaticCommandsEndpoint(UpdateDelegate next)
        {
            _next = next;
        }

        public Task Invoke(Update            update,
                           ContextDictionary contextDictionary,
                           IServiceProvider  provider,
                           PossibleCommands  possibleCommands,
                           Consumers         consumers)
        {
            var client = provider.GetService<IUpdateConsumer>();
            if (client is null)
            {
                throw new Exception("Client not found");
            }

            var commands = possibleCommands.Commands.OfType<IStaticCommand>().ToList();
            if (Initialize(commands.FirstOrDefault(t => t.SuitableFirst(update))))
            {
                return Task.CompletedTask;
            }

            var currentCommand = consumers.List.FirstOrDefault(t => !t.IsDone);
            if (currentCommand is not null)
            {
                currentCommand.Consume(update);
                contextDictionary.Add(update.GetId().Value, provider);
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

                contextDictionary.Add(update.GetId().Value, consumers);
                contextDictionary.Add(update.GetId().Value, provider);
                command = (IStaticCommand)provider.GetService(command.GetType())!;
                client.Initialize(command, update);
                consumers.List.AddFirst(client);
                return true;
            }

            return _next(update);
        }
    }
}