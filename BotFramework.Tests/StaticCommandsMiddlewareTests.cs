using System.Linq;
using BotFramework.Abstractions;
using BotFramework.HostServices;
using BotFramework.Middleware;
using EchoBot;
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
                        .ConfigureAppDebug(app =>
                        {
                            app.UseStaticCommands();
                            app.UseHandlers();
                        })
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
            var commands = _host.Services.GetServices(typeof(IStaticCommand)).Select(a => a.GetType()).ToList();

            Assert.Contains(typeof(EchoCommand), commands);
            Assert.Contains(typeof(HelpCommand), commands);
        }
    }
}