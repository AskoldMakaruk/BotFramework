using System.Linq;
using System.Reflection;
using BotFramework.Abstractions;
using BotFramework.HostServices;
using BotFramework.Middleware;
using EchoBot;
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
                            app.UseHandlers();
                        }, true)
                        .Build();
        }

        [Test]
        public void GetStaticCommands_WhenExecuted_ShouldReturnStaticCommands()
        {
            var commands = CommandsMiddlewareExtensions.GetStaticCommands().StaticCommandsTypes.ToList();

            Assert.Contains(typeof(EchoCommand), commands);
            Assert.Contains(typeof(HelpCommand), commands);
        }

        [Test]
        public void ServicesShouldContainStaticCommands()
        {
            var commands = _host.Services.GetService<StaticCommandsList>().StaticCommandsTypes.ToList();

            Assert.Contains(typeof(EchoCommand), commands);
            Assert.Contains(typeof(HelpCommand), commands);
        }
    }
}