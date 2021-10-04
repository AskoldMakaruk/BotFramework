using System;
using System.Threading.Tasks;
using BotFramework;
using BotFramework.Abstractions;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.HostServices;
using BotFramework.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Telegram.Bot;
using Telegram.Bot.Types;
using ILogger = Serilog.ILogger;

namespace EchoBot
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
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
                    app.UseHandlers();
                    app.UseStaticCommands();
                })
                // --| OR |--
                // use this
                .UseBotFrameworkStartup<Startup>()
                // --| OR |--
                // use this
                .UseBotFramework(new Startup().Configure, false)
                .Build()
                .Run();
        }
    }

    public class Startup : IStartup
    {
        public void Configure(IAppBuilder app, HostBuilderContext context)
        {
            app.Services.AddTelegramClient(context.Configuration["BotToken"]);
            app.Services.AddUpdateConsumer();
            app.UseMiddleware<LoggingMiddleware>();
            app.UseHandlers();
            app.UseStaticCommands();
        }

        public bool IsDebug => false;
    }

    public class EchoCommand : IStaticCommand
    {
        private readonly ILogger logger;

        public EchoCommand(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task Execute(IClient client)
        {
            var message = await client.GetTextMessage();

            logger.Information("DI Works!");

            await client.SendTextMessage($"Hello, here ypur last message {message.Text}, type somethinh again");

            message = await client.GetTextMessage();

            await client.SendTextMessage($"And this is your new message {message.Text}, and now type only message with hello");

            var helloMessage = await client.GetMessageWithHelloText();
            await client.SendTextMessage("Well done!");
        }

        public bool SuitableLast(Update context) => true;
    }

    public class HelpCommand : IStaticCommand
    {
        public async Task Execute(IClient client)
        {
            var _ = await client.GetTextMessage();
            await client.SendTextMessage("This is help text");
        }

        public bool SuitableFirst(Update ctx)
        {
            return ctx.Message?.Text == "/help";
        }
    }

    public static class Shit
    {
        public static async Task<Message> GetMessageWithHelloText(this IClient client)
        {
            return (await client.GetUpdate(u => u?.Message?.Text?.Contains("Hello") == true)).Message;
        }
    }
}