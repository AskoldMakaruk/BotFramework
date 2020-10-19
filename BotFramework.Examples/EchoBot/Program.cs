using BotFramework.Bot;
using BotFramework.BotTask;
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
            .WithToken("<YOURTOKEN>")
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
            await client.MakeRequestAsync(new SendMessageRequest(client.UserId, "Hello"));
            return Responses.Ok();
        }

        public bool Suitable(Update message)
        {
            return true;
        }
    }
}