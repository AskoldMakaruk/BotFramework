using System.Threading.Tasks;
using BotFramework.Abstractions;
using NUnit.Framework;
using BotFramework.Clients;
using BotFramework.HostServices;
using BotFramework.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace BotFramework.Tests
{
    public class Tests
    {
        private DebugClient client;


        [SetUp]
        public void Setup()
        {
            var host = Host.CreateDefaultBuilder()
                           .UseSerilog((context, configuration) =>
                           {
                               configuration
                               .MinimumLevel.Debug()
                               .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                               .Enrich.FromLogContext()
                               .WriteTo.Console();
                           })
                           .ConfigureHostConfiguration(builder => builder.AddEnvironmentVariables())
                           .UseSimpleBotFramework(true)
                           .Build();

            client = host.Services.GetService<IUpdateConsumer>() as DebugClient;
        }


        [Test]
        // [Timeout(5000)]
        public async Task Test1()
        {
            var text = "test text";
            await client.FromUser(
                new Message
                {
                    From = From,
                    Text = text
                });

            var message = await client.GetRequest<SendMessageRequest>();

            Assert.AreEqual($"Hello, here ypur last message {text}, type somethinh again", message.Text);

            await client.FromUser(
                new Message
                {
                    From = From,
                    Text = text
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