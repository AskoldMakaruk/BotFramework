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
        private UpdateDelegate app = null;
        private DebugClient    client;

        private IHost _host;

        [SetUp]
        public void Setup()
        {
            client = new DebugClient();
            _host = Host.CreateDefaultBuilder()
                        .UseSerilog((context, configuration) =>
                        {
                            configuration
                            .MinimumLevel.Debug()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                            .Enrich.FromLogContext()
                            .WriteTo.Console();
                        })
                        .ConfigureHostConfiguration(builder => builder.AddEnvironmentVariables())
                        .UseBotFramework((builder, _) =>
                        {
                            builder.Services.AddTransient<IUpdateConsumer, DebugClient>(_ => client);
                            builder.UseStaticCommands();
                            builder.UseHandlers();
                        }, true)
                        .Build();

            app = _host
                  .Services.GetService<AppRunnerServiceExtensions.DebugDelegateWrapper>()!.App;
        }


        [Test]
        //[Timeout(5000)]
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