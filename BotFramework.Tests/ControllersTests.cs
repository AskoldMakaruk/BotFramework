using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Extensions;
using BotFramework.Extensions.Hosting;
using BotFramework.Services.Clients;
using BotFramework.Services.Commands;
using BotFramework.Services.Commands.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace BotFramework.Tests.Middleware;

public class ControllersTests
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
                           (builder, context) => { builder.UseControllers(typeof(TestController)); }, true)
                       .Build();

        producer = host.Services.GetService<AppUpdateProducer>();
        _sink    = host.Services.GetService<IRequestSinc>() as MemorySink;
    }


    [Test]
    [Timeout(5000)]
    public async Task Test1()
    {
        var text = "/test_1234";
        await producer.FromUser(
            new Message
            {
                From = From,
                Text = text
            });

        var message = await _sink.GetRequest<SendMessageRequest>();

        Assert.AreEqual("start text", message.Text);
    }

    public static User From => new()
    {
        Id       = 1,
        Username = "UserName",
    };
}

[Command(priority: EndpointPriority.First)]
public class TestController : CommandControllerBase
{
    [Command("/test_1234")]
    public async Task Start()
    {
        await Client.SendMessageAsync("start text");
    }

    public TestController(IClient client, UpdateContext update) : base(client, update) { }
}
//
// public class StateController : PersistentControllerBase
// {
//     [PersistentState(1)]
//     public async Task Start()
//     {
//         await Client.SendMessageAsync("start text");
//     }
//     
// }