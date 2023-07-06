﻿using System.Collections.Generic;
using System.Threading.Tasks;
using BotFramework;
using BotFramework.Abstractions;
using BotFramework.Extensions;
using BotFramework.Extensions.Hosting;
using BotFramework.Hosting;
using BotFramework.Middleware;
using BotFramework.Services.Commands.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Telegram.Bot.Types;

namespace EchoBot;

public interface IA { }

public class a : IA { }

public class b : IA { }

public class c
{
    public c(IEnumerable<IA> @as) { }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
                       .UseConfigurationWithEnvironment()
                       .UseSerilog((context, configuration) =>
                       {
                           configuration
                           .MinimumLevel.Debug()
                           .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                           .Enrich.FromLogContext()
                           .WriteTo.Console();
                       })

                       // use this 
                       .UseSimpleBotFramework()

                       // --| OR |--
                       // use this
                       .UseBotFramework((app, context) =>
                       {
                           app.Services.AddTelegramClient(context.Configuration["BotToken"]);
                           app.Services.AddUpdateConsumer();
                           app.UseMiddleware<LoggingMiddleware>();
                           app.UseStaticCommands();

                           app.Services.AddScoped<IA, a>();
                           app.Services.AddScoped<IA, b>();
                           app.Services.AddScoped<c>();
                       })

                       // --| OR |--
                       // use this
                       .UseBotFrameworkStartup<Startup>()

                       // --| OR |--
                       // use this
                       .UseBotFramework(new Startup().Configure, false)
                       .Build();
        var c = host.Services.GetService<c>();

        host.Run();
    }
}

public class Startup : IStartup
{
    public void Configure(IAppBuilder app, HostBuilderContext context)
    {
        app.Services.AddTelegramClient(context.Configuration["BotToken"]);
        app.Services.AddUpdateConsumer();
        app.UseMiddleware<LoggingMiddleware>();
        app.UseStaticCommands();
    }

    public bool isTesting => false;
}

[Priority(EndpointPriority.Last)]
public class EchoCommand : ICommand
{
    private readonly ILogger logger;

    public EchoCommand(ILogger logger)
    {
        this.logger = logger;
    }

    public async Task Execute(IClient client) { }

    public bool SuitableLast(Update context) => true;

    public async Task Execute(UpdateContext context)
    {
        var client  = context.Client;
        var message = await client.GetTextMessage();

        logger.Information("DI Works!");

        await client.SendMessageAsync($"Hello, here ypur last message {message.Text}, type somethinh again");

        message = await client.GetTextMessage();

        await client.SendMessageAsync($"And this is your new message {message.Text}, and now type only message with hello");

        var helloMessage = await client.GetMessageWithHelloText();
        await client.SendMessageAsync("Well done!");
    }

    public bool? Suitable(UpdateContext context)
    {
        return true;
    }
}

public class HelpCommand : ICommand
{
    public async Task Execute(UpdateContext context)
    {
        var client = context.Client;
        var _      = await client.GetTextMessage();
        await client.SendMessageAsync("This is help text");
    }

    public bool? Suitable(UpdateContext ctx)
    {
        return ctx.Update.Message?.Text == "/help";
    }
}

public static class Shit
{
    public static async Task<Message> GetMessageWithHelloText(this IClient client)
    {
        return (await client.GetUpdate(u => u?.Message?.Text?.Contains("Hello") == true)).Message;
    }
}