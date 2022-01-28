using System;
using BotFramework.Abstractions;
using BotFramework.Middleware;
using BotFramework.Services.Controllers;

namespace BotFramework.Services.Factories
{
    public class EndpointFactory
    {
        private readonly IServiceProvider _provider;

        public EndpointFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IEndpoint CreateEndpoint(ICommand command, EndpointPriority priority)
        {
            if (command is ControllerEndpointCommand endpointCommand)
            {
                endpointCommand.ControllerIntance = _provider.GetService(endpointCommand.ControllerType)!;
            }

            var endpoint = (CommandEndpoint)_provider.GetService(typeof(CommandEndpoint))!;
            endpoint.Initlialize(command, priority);
            return endpoint;
        }
    }
}