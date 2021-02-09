using BotFramework.Abstractions;
using NUnit.Framework;
using BotFramework.Clients;
using BotFramework.Middleware;
using EchoBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Types;

namespace BotFramework.Tests
{
    public class Tests
    {
        private UpdateDelegate app = null;

        [SetUp]
        public void Setup()
        {
            using var host = Host.CreateDefaultBuilder()
                                 .ConfigureHostConfiguration(builder => builder.AddEnvironmentVariables())
                                 .ConfigureServices(services =>
                                 {
                                     services.AddTransient<IUpdateConsumer, DebugClient>();

                                     services.AddScoped<EchoCommand>();
                                     services.AddScoped<HelpCommand>();

                                     services.AddSingleton<ILogger, Logger>();
                                     services.AddScoped<DictionaryContext>();

                                     var builder = new AppBuilder(services.BuildServiceProvider());

                                     builder.UseStaticCommands();
                                     app = builder.Build();
                                 })
                                 .Build();
        }

        [Test]
        public void Test1()
        {
            app(new()
            {
                Message = new()
                {
                    From = From
                }
            });
        }

        public static User From => new()
        {
            Id       = 1,
            Username = "UserName",
        };
    }
}