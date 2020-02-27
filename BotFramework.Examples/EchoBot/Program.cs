using BotFramework.Bot;
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
            .WithToken("<YOURTOKEN>")
            .UseConsoleLogger()
            .Build()
            .Run();
        }
    }

    [StaticCommand]
    public class EchoCommand : MessageCommand
    {
        public override Response Execute(Message message, IGetOnlyClient client)
        {
            return new Response().AddMessage(new TextMessage(message.Chat.Id, message.Text));
        }

        public override bool Suitable(Message message) => true;
    }
}