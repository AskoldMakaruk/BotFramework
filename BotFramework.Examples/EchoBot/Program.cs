using System;
using System.Threading.Tasks;
using BotFramework;
using BotFramework.Abstractions;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.HostServices;
using BotFramework.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EchoBot
{
    internal class Program
    {
        private const string token = "1136669023:AAGujpEe6BmJgi5Wh3r_ncWE5ZX3nPK1WuE";

        private static void Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                                 .ConfigureHostConfiguration(builder => builder.AddEnvironmentVariables())
                                 .ConfigureApp(app =>
                                 {
                                     app.Services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(token));
                                     app.Services.AddTransient<IUpdateConsumer, Client>();
                                     app.Services.AddSingleton<ILogger, Logger>();
                                     app.UseHandlers();
                                     app.UseStaticCommands();
                                 })
                                 .Build()
                                 .RunAsync();
            Console.ReadLine();
        }
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

            logger.Log("DI Works!");

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

    public interface ILogger
    {
        void Log(string text);
    }

    public class Logger : ILogger
    {
        public void Log(string text)
        {
            Console.WriteLine(text);
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