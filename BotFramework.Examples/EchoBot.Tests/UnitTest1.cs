using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Extensions.Hosting;
using BotFramework.Services.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace EchoBot.Tests;

public class Tests
{
    private AppUpdateProducer producer;
    private MemorySink        _sink;

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
                       .UseSimpleBotFramework(
                           (builder, context) => { builder.UseStaticCommandsAssembly(typeof(HelpCommand).Assembly); }, true)
                       .Build();

        producer = host.Services.GetService<AppUpdateProducer>();
        _sink    = host.Services.GetService<IRequestSinc>() as MemorySink;
    }


    [Test]
    // [Timeout(5000)]
    public async Task Test1()
    {
        var text = "test text";
        await producer.FromUser(
            new Message
            {
                From = From,
                Text = text
            });

        var message = await _sink.GetRequest<SendMessageRequest>();

        Assert.AreEqual($"Hello, here ypur last message {text}, type somethinh again", message.Text);

        await producer.FromUser(
            new Message
            {
                From = From,
                Text = text
            });

        message = await _sink.GetRequest<SendMessageRequest>();
        Assert.AreEqual($"And this is your new message {text}, and now type only message with hello", message.Text);
    }

    public static User From => new()
    {
        Id       = 1,
        Username = "UserName",
    };
}