using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using BotFramework.Bot;
using BotFramework.Clients;
using BotFramework.Commands;
using BotFramework.Responses;
using Serilog;
using Serilog.Core;
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
                injector: new StupidInjector(),
                assembly: typeof(Program).Assembly,
                logger: logger)
            .CreateAndRunDictionaryInMemoryHandler();
        }
    }

    public class EchoCommand : IStaticCommand
    {
        public async Task<Response> Execute(IClient client)
        {
            //await Task.Delay(10000);
            var message = await client.GetTextMessageAsync();
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
            await client.SendTextMessageAsync("This is help text");
            return Responses.Ok();
        }

        public bool SuitableFirst(Update message)
        {
            return message?.Message?.Text == "/help";
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

    public class StupidInjector : IInjector
    {
        public ICommand Create(Type commandType)
        {
            if(typeof(HelpCommand) == commandType)
                return new HelpCommand();
            return new EchoCommand();
        }

        public T Create<T>() where T : ICommand
        {
            throw new NotImplementedException();
        }
    }
}