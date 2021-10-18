using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.Extensions;
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
        private AppUpdateProducer client;
        private MemorySink        _sink;


        [SetUp]
        public void Setup()
        {
            var host = Host.CreateDefaultBuilder()
                           .UseSerilog((context, configuration) =>
                           {
                               configuration
                               .MinimumLevel.Verbose()
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

            client = host.Services.GetService<AppUpdateProducer>();
            _sink  = host.Services.GetService<IRequestSinc>() as MemorySink;
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
                return update.Message.Text == nameof(CancelCommand);
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
            var message = GetMessage();
            await client.FromUser(message);
            (await _sink.GetRequest<SendMessageRequest>()).Text.Should().Contain("0");

            message.Text = "<any text>";
            await client.FromUser(message);
            (await _sink.GetRequest<SendMessageRequest>()).Text.Should().Contain("1");
        }

        [Test]
        public async Task StatefullCommand_WhenCancelMessagesReceived_ShoudDiscardState()
        {
            var message = GetMessage();

            await client.FromUser(message);
            (await _sink.GetRequest<SendMessageRequest>()).Text.Should().Contain("0");

            message      = GetMessage();
            message.Text = nameof(CancelCommand);

            await client.FromUser(message);
            (await _sink.GetRequest<SendMessageRequest>()).Text.Should().Be("Action was canceled");
        }

        private static Message GetMessage()
        {
            return new Message
            {
                From = new User
                {
                    Id       = 1,
                    Username = "UserName",
                },
                Text = "state"
            };
        }
    }
}