using System;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Extensions;
using Serilog.Core;
using Telegram.Bot.Types;
using ILogger = Serilog.ILogger;


namespace BotFramework.Middleware
{
    public enum EndpointPriority
    {
        First,
        Last
    }

    public interface IEndpoint
    {
        public EndpointPriority    Priority { get; }
        public Func<IClient, Task> Action   { get; }
    }

    public class CommandEndpoint : IEndpoint
    {
        private readonly ICommand _command;

        public CommandEndpoint(ICommand command, EndpointPriority priority)
        {
            Priority = priority;
            _command = command;
        }

        public EndpointPriority    Priority { get; }
        public Func<IClient, Task> Action   => _client => _command.Execute(_client);
    }

    public class SuitableMiddleware
    {
        private readonly UpdateDelegate _next;

        public SuitableMiddleware(UpdateDelegate next)
        {
            _next = next;
        }

        public Task Invoke(Update           update,
                           UpdateContext    updateContext,
                           IServiceProvider provider,
                           PossibleCommands possibleCommands,
                           ILogger?         logger = null)
        {
            logger ??= Logger.None;
            var commands = possibleCommands.Commands.OfType<IStaticCommand>().ToList();

            if (commands.FirstOrDefault(t => t.SuitableFirst(update)) is { } firstCommand)
            {
                logger.Debug("{Command} is first suitable to handle {User}'s request", firstCommand.GetType().Name,
                    update.GetUser());
                var command = (IStaticCommand)provider.GetService(firstCommand.GetType())!;
                updateContext.Endpoints.Add(new CommandEndpoint(command, EndpointPriority.First));
            }
            else if (commands.FirstOrDefault(t => t.SuitableLast(update)) is { } lastCommand)
            {
                logger.Debug("{Command} is last suitable to handle {User}'s request", lastCommand.GetType().Name,
                    update.GetUser());
                var command = (IStaticCommand)provider.GetService(lastCommand.GetType())!;
                updateContext.Endpoints.Add(new CommandEndpoint(command, EndpointPriority.Last));
            }
            else
            {
                logger.Debug("No suitable command found for {User}", update.GetUser());
            }


            return _next(update);
        }
    }
}