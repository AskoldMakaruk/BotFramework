using System;
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
            .WithName("EchoBot")
            .WithToken("<YOUR TOKEN>")
            .UseLogger((c, m) => Console.WriteLine(m))
            .Build()
            .Run();
        }
    }

    public class EchoCommand : MessageCommand
    {
        public override Response Execute(Message message, Client client)
        {
            return new Response().AddMessage(new TextMessage(message.Chat.Id, message.Text));
        }

        public override bool Suitable(Message message) => true; //accept any message
    }
}