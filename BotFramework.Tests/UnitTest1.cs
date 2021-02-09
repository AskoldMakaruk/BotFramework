using System.Threading.Tasks;
using BotFramework.Abstractions;
using NUnit.Framework;
using BotFramework.Clients;
using BotFramework.Middleware;
using EchoBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace BotFramework.Tests
{
    public class Tests
    {
        private UpdateDelegate app = null;
        private DebugClient    client;

        [SetUp]
        public void Setup()
        {
            client = new DebugClient();
            using var host = Host.CreateDefaultBuilder()
                                 .ConfigureHostConfiguration(builder => builder.AddEnvironmentVariables())
                                 .ConfigureServices(services =>
                                 {
                                     services.AddTransient<IUpdateConsumer, DebugClient>(_ => client);

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
        public async Task Test1()
        {
            var text = "test text";
            await app(new()
            {
                Message = new()
                {
                    From = From,
                    Text = text
                }
            });
            var message = await client.GetRequest<SendMessageRequest>();
            Assert.AreEqual($"Hello, here ypur last message {text}, type somethinh again", message.Text);

            await app(new()
            {
                Message = new()
                {
                    From = From,
                    Text = text
                }
            });

            message = await client.GetRequest<SendMessageRequest>();
            Assert.AreEqual($"And this is your new message {text}, and now type only message with hello", message.Text);
        }

        public static User From => new()
        {
            Id       = 1,
            Username = "UserName",
        };
    }
}