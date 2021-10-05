using System;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Helpers;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Telegram.Bot.Types;
using ILogger = Serilog.ILogger;

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
                           IUpdateConsumer   client,
                           ILogger?          logger = null)
        {
            logger ??= Logger.None;
            var id       = update.GetId().Value;
            var commands = possibleCommands.Commands.OfType<IStaticCommand>().ToList();

            var command = commands.FirstOrDefault(t => t.SuitableFirst(update));
            if (Initialize(command))
            {
                logger.Debug("{Command} is first suitable to handle {User}'s request", command.GetType().Name,
                    update.GetUser());
                return Task.CompletedTask;
            }

            var currentCommand = consumers.List.FirstOrDefault(t => !t.IsDone);
            if (currentCommand is not null)
            {
                logger.Debug("Current command is handling {User}'s request",
                    update.GetUser());

                currentCommand.Consume(update);
                contextDictionary.Add(id, provider);
                return Task.CompletedTask;
            }

            command = commands.FirstOrDefault(t => t.SuitableLast(update));
            if (Initialize(command))
            {
                logger.Debug("{Command} is last suitable to handle {User}'s request", command.GetType().Name,
                    update.GetUser());
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

            logger.Debug("No suitable command found for {User}", update.GetUser());
            return _next(update);
        }
    }
}