using System;
using System.Threading.Tasks;
using BotFramework.Bot;
using BotFramework.Clients;
using BotFramework.Commands;
using BotFramework.Responses;
using Telegram.Bot.Types;

namespace EchoBot
{
    class Program
    {
        static void Main()
        {
            new BotBuilder()
            .UseAssembly(typeof(Program).Assembly)
            .WithToken("547180886:AAGzSudnS64sVfN2h6hFZTqjkJsGELfEVKQ")
            .WithInjector(new StupidInjector())
            .UseConsoleLogger()
            .Build()
            .Run();
        }
    }

    [StaticCommand]
    public class EchoCommand : IOnStartCommand
    {
        public async Task<Response> Execute(IClient client)
        {
            await Task.Delay(10000);
            var message = await client.GetTextMessageAsync();
            await client.SendTextMessageAsync($"Hello, here ypur last message {message.Text}, type somethinh again");
            message = await client.GetTextMessageAsync();
            await client.SendTextMessageAsync($"And this is your new message {message.Text}, and now type only message with hello");
            var helloMessage = await client.GetMessageWithHelloTextAsync();
            await client.SendTextMessageAsync("Well done!");
            return Responses.Ok();
        }


        public bool Suitable(Update message)
        {
            return true;
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
            return new EchoCommand();
        }

        public T Create<T>() where T : ICommand
        {
            throw new NotImplementedException();
        }
    }
}