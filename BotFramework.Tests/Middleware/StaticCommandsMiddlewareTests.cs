using System;
using System.Linq;
using BotFramework.Extensions;
using BotFramework.HostServices;
using BotFramework.Middleware;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace BotFramework.Tests
{
    public class StaticCommandsMiddlewareTests
    {
        private IHost _host;

        [SetUp]
        public void Setup()
        {
            _host = Host.CreateDefaultBuilder()
                        .ConfigureHostConfiguration(builder => builder.AddEnvironmentVariables())
                        .UseBotFramework((app, _) =>
                        {
                            app.UseStaticCommands();
                            //app.UseHandlers();
                        }, true)
                        .Build();
        }

        private Type[] commandTypes =
        {
            typeof(StateMachineTests.StatefullCommand)
        };

        // [Test]
        // public void GetStaticCommands_WhenExecuted_ShouldReturnStaticCommands()
        // {
        //     var commands = CommandsMiddlewareExtensions.GetAssemblies().StaticCommandsTypes.ToList();
        //     commands.Should().Contain(commandTypes);
        // }

        [Test]
        public void ServicesShouldContainStaticCommands()
        {
            var commands = _host.Services.GetService<StaticCommandsList>()!.StaticCommandsTypes.ToList();
            commands.Should().Contain(commandTypes);
        }
    }
}