using System;
using BotFramework.Bot;
using BotFramework.BotTask;
using BotFramework.Clients;
using BotFramework.Commands;
using BotFramework.Responses;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace EchoBot
{
    class Program
    {
        static void Main()
        {
            new BotBuilder()
            .UseAssembly(typeof(Program).Assembly)
            .WithToken("")
            .WithInjector(new StupidInjector())
            .UseConsoleLogger()
            .Build()
            .Run();
        }
    }

    [StaticCommand]
    public class EchoCommand : IOnStartCommand
    {
        public async BotTask<Response> Execute(IClient client)
        {
            var update = await client.GetUpdateAsync();
            await client.MakeRequestAsync(new SendMessageRequest(client.UserId, $"Hello, here ypur last message {update.Message.Text}, type somethinh again"));
            update = await client.GetUpdateAsync();
            await client.MakeRequestAsync(new SendMessageRequest(client.UserId, $"And this is your new message {update.Message.Text}, and now type only message with hello"));
            var helloMessage = await client.GetMessageWithHelloTextAsync();
            await client.MakeRequestAsync(new SendMessageRequest(client.UserId, $"Well done!"));
            return Responses.Ok();
        }


        public bool Suitable(Update message)
        {
            return true;
        }
    }

    public static class Shit
    {
        public static async BotTask<Message> GetMessageWithHelloTextAsync(this IClient client)
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