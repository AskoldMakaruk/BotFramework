using System.Reflection;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.HostServices;
using BotFramework.Middleware;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace BotFramework.Tests
{
    public class StateMachineTests
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
                           .UseSimpleBotFramework((builder, context) =>
                           {
                               builder.UseStaticCommands(new StaticCommandsList(new[]
                               {
                                   typeof(CancelCommand),
                                   typeof(StatefullCommand)
                               }));
                           }, true)
                           .Build();

            client = host.Services.GetService<IUpdateConsumer>() as DebugClient;
        }

        public class StatefullService
        {
            private int _state;
            public  int State => _state++;
        }

        public class CancelCommand : IStaticCommand
        {
            public bool SuitableFirst(Update update)
            {
                return update.Message.Text == "StateMachineTests.Cancel";
            }

            public async Task Execute(IClient client)
            {
                await client.SendTextMessage("Action was canceled");
            }
        }

        public class StatefullCommand : IStaticCommand
        {
            private readonly StatefullService _service;

            public StatefullCommand()
            {
                _service = new();
            }

            public bool SuitableFirst(Update update)
            {
                return update.Message.Text == "state";
            }

            public async Task Execute(IClient client)
            {
                var _ = await client.GetTextMessage();

                await client.SendTextMessage($"State is {_service.State}");

                _ = await client.GetTextMessage();
                await client.SendTextMessage($"State is {_service.State}");
            }
        }

        [Test]
        public async Task StatefullCommand_WhenMultipleMessagesReceived_ShoudPreserveState()
        {
            var message = new Message
            {
                From = new User
                {
                    Id       = 1,
                    Username = "UserName",
                },
                Text = "state"
            };

            await client.FromUser(message);
            (await client.GetRequest<SendMessageRequest>()).Text.Should().Contain("0");

            message.Text = "<any text>";
            await client.FromUser(message);
            (await client.GetRequest<SendMessageRequest>()).Text.Should().Contain("1");
        }

        [Test]
        public async Task StatefullCommand_WhenCancelMessagesReceived_ShoudDiscardState()
        {
            var message = new Message
            {
                From = new User
                {
                    Id       = 1,
                    Username = "UserName",
                },
                Text = "state"
            };

            await client.FromUser(message);
            (await client.GetRequest<SendMessageRequest>()).Text.Should().Contain("0");

            message.Text = "StateMachineTests.Cancel";
            await client.FromUser(message);
            (await client.GetRequest<SendMessageRequest>()).Text.Should().Be("Action was canceled");
        }
    }
}