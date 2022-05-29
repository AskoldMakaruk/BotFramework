using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Extensions;
using BotFramework.Extensions.Hosting;
using BotFramework.Middleware;
using BotFramework.Services.Clients;
using BotFramework.Services.Commands.Attributes;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace BotFramework.Tests;

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
                           builder.UseStaticCommands(new List<Type>
                           {
                               typeof(CancelCommand),
                               typeof(StatefullCommand)
                           });
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

    [Command(priority: EndpointPriority.First)]
    public class CancelCommand : ICommand
    {
        public async Task Execute(UpdateContext context)
        {
            await context.Client.SendTextMessage("Action was canceled");
        }

        public bool? Suitable(UpdateContext context)
        {
            return context.Update.Message?.Text == nameof(CancelCommand);
        }
    }

    [Command(priority: EndpointPriority.Last)]
    public class StatefullCommand : ICommand
    {
        private readonly StatefullService _service;

        public StatefullCommand()
        {
            _service = new();
        }

        public async Task Execute(UpdateContext context)
        {
            var _ = await context.Client.GetTextMessage();

            await context.Client.SendTextMessage($"State is {_service.State}");

            _ = await context.Client.GetTextMessage();
            await context.Client.SendTextMessage($"State is {_service.State}");
        }

        public bool? Suitable(UpdateContext context)
        {
            return context.Update.Message?.Text == "state";
        }
    }

    [Test]
    [Timeout(5000)]
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
    [Timeout(5000)]
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