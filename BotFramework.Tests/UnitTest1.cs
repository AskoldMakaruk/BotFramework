using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using NUnit.Framework;
using BotFramework.Clients;
using BotFramework.HostServices;
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

        private IHost _host;

        [SetUp]
        public void Setup()
        {
            client = new DebugClient();
            _host = Host.CreateDefaultBuilder()
                        .ConfigureHostConfiguration(builder => builder.AddEnvironmentVariables())
                        .ConfigureAppDebug(app =>
                        {
                            app.Services.AddTransient<IUpdateConsumer, DebugClient>(_ => client);
                            app.UseStaticCommands();
                            app.UseHandlers();
                        })
                        .Build();

            app = _host
                  .Services.GetService<AppRunnerServiceExtensions.DebugDelegateWrapper>()!.App;
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
            var cmd      = new EchoCommand(null);
            var commands = _host.Services.GetServices(typeof(IStaticCommand)).Select(a => a.GetType()).ToList();

            Assert.Contains(typeof(EchoCommand), commands);
            Assert.Contains(typeof(HelpCommand), commands);
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