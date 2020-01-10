using BotFramework.Bot;
using BotFramework.Commands;
using BotFramework.Responses;
using Serilog;
using Telegram.Bot.Types;

namespace EchoBot
{
    class Program
    {
        static void Main()
        {
            new BotBuilder()
            .UseAssembly(typeof(Program).Assembly)
            .WithName("EchoBot")
            //.WithToken("<YOURTOKEN>")
            .WithToken("823973981:AAGYpq1Eyl_AAYGXLeW8s28uCH89S7fsHZA")
            .UseLogger(new LoggerConfiguration()
                       .MinimumLevel.Debug()
                       .WriteTo.Console()
                       .CreateLogger())
            .Build()
            .Run();
        }
    }

    public class EchoCommand : MessageCommand, IStaticCommand
    {
        public override Response Execute(Message message, Client client)
        {
            return new Response(new TextMessage(message.Chat.Id, message.Text));
        }

        public override bool Suitable(Message message) => true;
    }
}