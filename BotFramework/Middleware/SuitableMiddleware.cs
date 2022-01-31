using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Services.Factories;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace BotFramework.Middleware
{
    public class CommandEndpoint : IEndpoint
    {
        public EndpointPriority Priority { get; private set; }
        public Task             Action   => _command.Execute(_client);
        public string[]?        Claims   { get; set; }

        private readonly IClient  _client;
        private          ICommand _command;

        public CommandEndpoint(IClient client)
        {
            _client = client;
        }

        public void Initlialize(ICommand command, EndpointPriority priority, string[]? claims)
        {
            Priority = priority;
            _command = command;
            Claims   = claims;
        }
    }

    public class SuitableMiddleware
    {
        private readonly UpdateDelegate _next;

        public SuitableMiddleware(UpdateDelegate next)
        {
            _next = next;
        }

        public Task Invoke(Update                       update,
                           UpdateContext                updateContext,
                           PossibleCommands             possibleCommands,
                           EndpointFactory              endpointFactory,
                           ILogger<SuitableMiddleware>? logger = null)
        {
            var commands = possibleCommands.Commands.OfType<IStaticCommand>().ToList();

            if (commands.FirstOrDefault(t => t.SuitableFirst(update)) is { } firstCommand)
            {
                logger?.LogDebug("{Command} is first suitable to handle {User}'s request", firstCommand.GetType().Name,
                    update.GetUser());

                updateContext.Endpoints.Add(endpointFactory.CreateEndpoint(firstCommand, EndpointPriority.First));
            }

            if (commands.FirstOrDefault(t => t.SuitableLast(update)) is { } lastCommand)
            {
                logger?.LogDebug("{Command} is last suitable to handle {User}'s request", lastCommand.GetType().Name,
                    update.GetUser());

                updateContext.Endpoints.Add(endpointFactory.CreateEndpoint(lastCommand, EndpointPriority.Last));
            }

            // else
            // {
            //     logger.Debug("No suitable command found for {User}", update.GetUser());
            // }


            return _next(update);
        }
    }
}