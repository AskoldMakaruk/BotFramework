using System;
using System.Threading.Tasks;
using BotFramework;
using BotFramework.Abstractions;
using BotFramework.Clients.ClientExtensions;
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
            UpdateDelegate app = null;
            using var host = Host.CreateDefaultBuilder(args)
                                 .ConfigureHostConfiguration(builder => builder.AddEnvironmentVariables())
                                 .ConfigureServices(services =>
                                 {
                                     services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(token));
                                     services.AddScoped<EchoCommand>();
                                     services.AddScoped<HelpCommand>();
                                     services.AddSingleton<ILogger, Logger>();
                                     services.AddScoped<DictionaryContext>();
                                     var builder = new AppBuilder(services.BuildServiceProvider());


                                     builder.UseStaticCommands(new StaticCommandsList(new()
                                     {typeof(EchoCommand), typeof(HelpCommand)}));
                                     app = builder.Build();
                                 })
                                 .Build();
            var bot = host.Services.GetService<ITelegramBotClient>()!;
            bot!.OnUpdate += (sender, eventArgs) => app(eventArgs.Update);
            bot.StartReceiving();
            Console.ReadLine();
        }
    }
    
    public class EchoCommand : StaticCommand
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