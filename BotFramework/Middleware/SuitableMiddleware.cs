using System;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Helpers;
using Telegram.Bot.Types;

namespace BotFramework.Middleware
{
    public class SuitableMiddleware
    {
        private readonly UpdateDelegate _next;

        public SuitableMiddleware(UpdateDelegate next)
        {
            _next = next;
        }

        public Task Invoke(Update            update,
                           ContextDictionary contextDictionary,
                           IServiceProvider  provider,
                           PossibleCommands  possibleCommands,
                           Consumers         consumers,
                           IUpdateConsumer   client)
        {
            var id       = update.GetId().Value;
            var commands = possibleCommands.Commands.OfType<IStaticCommand>().ToList();

            if (Initialize(commands.FirstOrDefault(t => t.SuitableFirst(update))))
            {
                return Task.CompletedTask;
            }

            var currentCommand = consumers.List.FirstOrDefault(t => !t.IsDone);
            if (currentCommand is not null)
            {
                currentCommand.Consume(update);
                contextDictionary.Add(id, provider);
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

                contextDictionary.Add(id, consumers);
                contextDictionary.Add(id, provider);
                
                command = (IStaticCommand)provider.GetService(command.GetType())!;
                client.Initialize(command, update);
                
                consumers.List.AddFirst(client);
                return true;
            }

            return _next(update);
        }
    }
}