using System;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Extensions.Hosting;
using BotFramework.Services.Clients;
using BotFramework.Services.Extensioins;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace BotFramework.Identity.Tests;

public class IdentityTests
{
    private AppUpdateProducer client;
    private MemorySink        _sink;

    private Mock<UserManager<IdentityUser>> _userRepository;

    private readonly string _username = "UserName";

    [SetUp]
    public void Setup()
    {
        _userRepository = new Mock<UserManager<IdentityUser>>();

        var host = Host.CreateDefaultBuilder()
                       .ConfigureHostConfiguration(builder => builder.AddEnvironmentVariables())
                       .UseSimpleBotFramework((builder, context) =>
                       {
                           builder.UseIdentity<IdentityUser>();
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

    public class IdentityCommand : ICommand
    {
        private readonly IdentityUser _identityUser;

        public IdentityCommand(IdentityUser identityUser)
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


    [Test]
    public async Task IdentityMiddleware_WhenUserExists_ShouldInjectUserIntoDI()
    {
        var message = Message();

        _userRepository.Setup(repository => repository.FindByIdAsync(1))
                       .ReturnsAsync(() => new IdentityUser() { Id = 1, UserName = _username });

        await client.FromUser(message);
        (await _sink.GetRequest<SendMessageRequest>()).Text.Should().Contain(_username);
    }

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