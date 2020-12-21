using System;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Clients.ClientExtensions;
using BotFramework.Commands;
using BotFramework.Handlers;
using BotFramework.Responses;
using Ninject.Modules;
using Telegram.Bot.Types;

namespace EchoBot
{
    class Program
    {
        static void Main()
        {
            new HandlerConfigurationBuilder(token: "547180886:AAGzSudnS64sVfN2h6hFZTqjkJsGELfEVKQ")
            .WithCustomNinjectModules(new DumbModule())
            .UseConsoleDefaultLogger()
            .Build()
            .RunInMemoryHandler();
        }
    }

    public class EchoCommand : IStaticCommand
    {
        private readonly ILogger logger;

        public EchoCommand(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task<Response> Execute(IClient client)
        {
            var message = await client.GetTextMessage();

            logger.Log("DI Works!");

            await client.SendTextMessage($"Hello, here ypur last message {message.Text}, type somethinh again");

            message = await client.GetTextMessage();

            await client.SendTextMessage($"And this is your new message {message.Text}, and now type only message with hello");

            var helloMessage = await client.GetMessageWithHelloText();
            await client.SendTextMessage("Well done!");

            return Responses.Ok();
        }

        public bool SuitableLast(Update message) => true;
    }

    public class HelpCommand : IStaticCommand
    {
        public async Task<Response> Execute(IClient client)
        {
            var _ = await client.GetTextMessage();
            await client.SendTextMessage("This is help text");
            return Responses.Ok();
        }

        public bool SuitableFirst(Update message)
        {
            return message?.Message?.Text == "/help";
        }
    }

    public class DumbModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogger>().To<Logger>();
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