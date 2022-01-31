using System;
using System.Reflection;
using BotFramework.Abstractions;
using BotFramework.Authorization;
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
            else
            {
                command = (IStaticCommand)_provider.GetService(command.GetType())!;
            }

            var endpoint = (CommandEndpoint)_provider.GetService(typeof(CommandEndpoint))!;
            endpoint.Initlialize(command, priority, GetClaims(command.GetType()));
            return endpoint;
        }

        private string[]? GetClaims(MemberInfo command)
        {
            if (command.GetCustomAttribute(typeof(AuthorizeAttribute), true) is AuthorizeAttribute attr)
            {
                return attr.Claims;
            }

            return null;
        }
    }
}