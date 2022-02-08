using System;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Extensions;
using BotFramework.Extensions.Hosting;
using BotFramework.Services.Clients;
using BotFramework.Services.Commands;
using BotFramework.Services.Commands.Attributes;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace BotFramework.Identity.Tests;

public class IdentityTests
{
    private AppUpdateProducer client;
    private MemorySink        _sink;

    private Mock<UserManager<PersistentUser>> _userRepository;

    private readonly string _username = "UserName";

    [SetUp]
    public void Setup()
    {
        _userRepository = new Mock<UserManager<PersistentUser>>();

        var host = Host.CreateDefaultBuilder()
                       .ConfigureHostConfiguration(builder => builder.AddEnvironmentVariables())
                       .UseSimpleBotFramework((builder, context) =>
                       {
                           builder.AddIdentityWithPersistence<PersistentUser, IdentityRole>();
                           builder.Services.AddSingleton(provider => _userRepository.Object);
                       }, true)
                       .UseSerilog((context, configuration) =>
                       {
                           configuration.MinimumLevel.Debug()
                                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                                        .WriteTo.Console();
                       })
                       .Build();

        client = host.Services.GetService<AppUpdateProducer>();
        _sink  = host.Services.GetService<IRequestSinc>() as MemorySink;
    }

    public class PersistentUser : IdentityUser, IUserCommandState
    {
        public long   UserId       { get; set; }
        public string EndpointName { get; set; }
        public int    State        { get; set; }
    }

    public class IdentityCommand : ICommand
    {
        private readonly PersistentUser _identityUser;

        public IdentityCommand(PersistentUser identityUser)
        {
            _identityUser = identityUser;
        }


        public async Task Execute(UpdateContext context)
        {
            await context.Client.SendTextMessage(_identityUser.UserName);
        }

        public bool? Suitable(UpdateContext context)
        {
            return context.Update.Message?.Text == nameof(IdentityCommand);
        }
    }

    
    
    [Priority(EndpointPriority.First)]
    public class StateCommandController : PersistentControllerBase
    {
        public StateCommandController(UpdateContext context, IClient client) : base(client,
            context) { }

        [PersistentState(1)]
        public async Task State_1()
        {
            await Client.SendTextMessage("1");
            SetState(2);
        }

        [PersistentState(2)]
        public async Task State_2()
        {
            await Client.SendTextMessage("2");
            SetState(1);
        }
    }


    [Test]
    public async Task IdentityMiddleware_WhenUserExists_ShouldInjectUserIntoDI()
    {
        var message = Message();

        _userRepository.Setup(repository => repository.FindByIdAsync(1))
                       .ReturnsAsync(() => new() { Id = 1, UserName = _username });

        await client.FromUser(message);
        (await _sink.GetRequest<SendMessageRequest>()).Text.Should().Contain(_username);
    }

    [Test]
    public async Task StateTests()
    {
        var message = Message();
        message.Text = "";
        _userRepository.Setup(repository => repository.FindByIdAsync(1))
                       .ReturnsAsync(() => new()
                       { Id = 1, UserName = _username, State = 1, EndpointName = "StateCommandController" });

        await client.FromUser(message);
        (await _sink.GetRequest<SendMessageRequest>()).Text.Should().Contain("1");
        
        
        _userRepository.Setup(repository => repository.FindByIdAsync(1))
                       .ReturnsAsync(() => new()
                       { Id = 1, UserName = _username, State = 2, EndpointName = "StateCommandController" });
        
        await client.FromUser(message);
        (await _sink.GetRequest<SendMessageRequest>()).Text.Should().Contain("2");
    }

    //
    // [Test]
    // public async Task IdentityMiddleware_WhenUserDoesntExists_ShouldCreateUser()
    // {
    //     var message = Message();
    //
    //     _userRepository.Setup(repository => repository.FindByIdAsync(1)).ReturnsAsync(() => null);
    //     _userRepository.Setup(repository => repository.CreateAsync(message.From))
    //                    .ReturnsAsync(() => new IdentityUser(1, _username));
    //
    //     await client.FromUser(message);
    //     (await _sink.GetRequest<SendMessageRequest>()).Text.Should().Contain(_username);
    // }
    //
    // [Test]
    // [Ignore("Fix this")]
    // public async Task IdentityMiddleware_WhenUserCannotBeCreated_ShouldThrow()
    // {
    //     var message = Message();
    //
    //     _userRepository.Setup(repository => repository.GetUser(1)).ReturnsAsync(() => null);
    //     _userRepository.Setup(repository => repository.CreateUser(message.From))
    //                    .ReturnsAsync(() => null);
    //
    //     await client.FromUser(message);
    //     _sink.Invoking(a => a.GetRequest<SendMessageRequest>())
    //          .Should()
    //          .Throw<NullReferenceException>();
    //     await _sink.GetRequest<SendMessageRequest>();
    // }

    private Message Message()
    {
        var message = new Message
        {
            From = new User
            {
                Id       = 1,
                Username = _username,
            },
            Text = nameof(IdentityCommand)
        };
        return message;
    }
}