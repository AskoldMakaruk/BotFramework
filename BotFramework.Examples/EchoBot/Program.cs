using System;
using System.Threading.Tasks;
using BotFramework.Bot;
using BotFramework.Clients;
using BotFramework.Commands;
using BotFramework.Responses;
using Ninject.Modules;
using Serilog;
using Telegram.Bot.Types;

namespace EchoBot
{
    class Program
    {
        static void Main()
        {
            var logger = new LoggerConfiguration()
                         .MinimumLevel.Debug()
                         .WriteTo.Console()
                         .Enrich.FromLogContext()
                         .CreateLogger();
            new HandlerBuilder(token: "547180886:AAGzSudnS64sVfN2h6hFZTqjkJsGELfEVKQ",
                assembly: typeof(Program).Assembly,
                withCustomModules: modules => modules.Add(new DumbModule()),
                logger: logger)
            .CreateAndRunDictionaryInMemoryHandler();
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
            //await Task.Delay(10000);
            var message = await client.GetTextMessageAsync();
            logger.Log("DI Works!");
            await client.SendTextMessageAsync($"Hello, here ypur last message {message.Text}, type somethinh again");
            message = await client.GetTextMessageAsync();
            await client.SendTextMessageAsync(
                $"And this is your new message {message.Text}, and now type only message with hello");
            var helloMessage = await client.GetMessageWithHelloTextAsync();
            await client.SendTextMessageAsync("Well done!");
            return Responses.Ok();
        }


        public bool SuitableLast(Update message) => true;
    }
    public class HelpCommand : IStaticCommand
    {
        public async Task<Response> Execute(IClient client)
        {
            var _ = await client.GetTextMessageAsync();
            await client.SendTextMessageAsync("This is help text");
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
        public static async Task<Message> GetMessageWithHelloTextAsync(this IClient client)
        {
            var res = await client.GetUpdateAsync(u => u?.Message?.Text?.Contains("Hello") == true);
            return res.Message;
        }
    }

}