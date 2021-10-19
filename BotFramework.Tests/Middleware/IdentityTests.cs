using System;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Extensions.Hosting;
using BotFramework.Middleware;
using BotFramework.Services.Clients;
using BotFramework.Services.Extensioins;
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

namespace BotFramework.Tests.Middleware
{
    public class IdentityTests
    {
        public record IdentityUser(long Id, string Name);

        private AppUpdateProducer client;
        private MemorySink        _sink;

        private Mock<IUserRepository<IdentityUser>> _userRepository;

        private readonly string _username = "UserName";

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository<IdentityUser>>();

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
                               builder.UseIdentity<IdentityUser>();
                               builder.Services.AddSingleton(provider => _userRepository.Object);
                           }, true)
                           .Build();

            client = host.Services.GetService<AppUpdateProducer>();
            _sink  = host.Services.GetService<IRequestSinc>() as MemorySink;
        }

        public class IdentityCommand : IStaticCommand
        {
            private readonly IdentityUser _identityUser;

            public IdentityCommand(IdentityUser identityUser)
            {
                _identityUser = identityUser;
            }

            public bool SuitableFirst(Update update)
            {
                return update.Message.Text == nameof(IdentityCommand);
            }

            public async Task Execute(IClient client)
            {
                await client.SendTextMessage(_identityUser.Name);
            }
        }


        [Test]
        public async Task IdentityMiddleware_WhenUserExists_ShouldInjectUserIntoDI()
        {
            var message = Message();

            _userRepository.Setup(repository => repository.GetUser(1)).ReturnsAsync(() => new IdentityUser(1, _username));

            await client.FromUser(message);
            (await _sink.GetRequest<SendMessageRequest>()).Text.Should().Contain(_username);
        }

        [Test]
        public async Task IdentityMiddleware_WhenUserDoesntExists_ShouldCreateUser()
        {
            var message = Message();

            _userRepository.Setup(repository => repository.GetUser(1)).ReturnsAsync(() => null);
            _userRepository.Setup(repository => repository.CreateUser(message.From))
                           .ReturnsAsync(() => new IdentityUser(1, _username));

            await client.FromUser(message);
            (await _sink.GetRequest<SendMessageRequest>()).Text.Should().Contain(_username);
        }

        [Test]
        [Ignore("Fix this")]
        public async Task IdentityMiddleware_WhenUserCannotBeCreated_ShouldThrow()
        {
            var message = Message();

            _userRepository.Setup(repository => repository.GetUser(1)).ReturnsAsync(() => null);
            _userRepository.Setup(repository => repository.CreateUser(message.From))
                           .ReturnsAsync(() => null);

            await client.FromUser(message);
            _sink.Invoking(a => a.GetRequest<SendMessageRequest>())
                 .Should()
                 .Throw<NullReferenceException>();
            await _sink.GetRequest<SendMessageRequest>();
        }

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
}