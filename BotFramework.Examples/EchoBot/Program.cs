using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BotFramework;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.Commands;
using BotFramework.Helpers;
using BotFramework.Injectors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EchoBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var token = "1136669023:AAGujpEe6BmJgi5Wh3r_ncWE5ZX3nPK1WuE";
            using var host = Host.CreateDefaultBuilder(args)
                                 .ConfigureHostConfiguration(builder => builder.AddEnvironmentVariables())
                                 .ConfigureServices(services =>
                                 {
                                     services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(token));
                                     services.AddScoped<EchoCommand>();
                                     services.AddScoped<HelpCommand>();
                                     services.AddSingleton<ILogger, Logger>();
                                     services.AddSingleton<IInjector, MicrosoftInjector>();
                                     var builder = new AppBuilder<MyContext>(new MicrosoftInjectorBuilder(services));
                                     builder.UseSingleton<DictionaryCreatorMiddleware<MyContext>>();
                                     builder.UseSingleton<SuitableFirstMiddleware<MyContext>>();
                                     builder.UseSingleton<SuitableLastMiddleware<MyContext>>();
                                     builder.UseSingleton<EndPointMiddleware<MyContext>>();
                                     builder.AddContextCreator(update => new MyContext()
                                     {
                                         ChatId = update.GetUser(),
                                         StaticCommands = new () {typeof(EchoCommand), typeof(HelpCommand)},
                                         CurrentUpdate = update
                                     });
                                     var app = builder.Build();
                                     services.AddSingleton(_ => app);
                                 })
                                 .Build();
            var app = host.Services.GetService<IApp>()!;
            var bot = host.Services.GetService<ITelegramBotClient>()!;
            bot!.OnUpdate += (sender, eventArgs) => app!.Run(eventArgs.Update);
            bot.StartReceiving();
            Console.ReadLine();

        }
    }

    public class MyContext : IStaticCommandsContext
    {
        public User                        ChatId         { get; init; }
        public Update                      CurrentUpdate  { get; init; }
        public LinkedList<IUpdateConsumer> Handlers       { get; set; }
        public List<Type>                  StaticCommands { get; init; }
    }

    public class EchoCommand : IStaticCommand<MyContext>
    {
        private readonly ILogger logger;

        public EchoCommand(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task Execute(IClient client, MyContext _)
        {
            var message = await client.GetTextMessage();

            logger.Log("DI Works!");

            await client.SendTextMessage($"Hello, here ypur last message {message.Text}, type somethinh again");

            message = await client.GetTextMessage();

            await client.SendTextMessage($"And this is your new message {message.Text}, and now type only message with hello");

            var helloMessage = await client.GetMessageWithHelloText();
            await client.SendTextMessage("Well done!");
        }

        public bool SuitableLast(MyContext context) => true;
    }

    public class HelpCommand : IStaticCommand<MyContext>
    {
        public async Task Execute(IClient client, MyContext ctx)
        {
            var _ = await client.GetTextMessage();
            await client.SendTextMessage("This is help text");
        }

        public bool SuitableFirst(MyContext ctx)
        {
            return ctx.CurrentUpdate?.Message?.Text == "/help";
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