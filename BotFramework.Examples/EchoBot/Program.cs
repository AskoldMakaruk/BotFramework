using System;
using BotFramework.Bot;
using BotFramework.Commands;
using BotFramework.Commands.Validators;
using BotFramework.Responses;
using Optional;
using Telegram.Bot.Types;

namespace EchoBot
{
    class Program
    {
        static void Main()
        {
            var configuration = new BotConfiguration(typeof(Program).Assembly, "<YOUR TOKEN>")
            {
                Logger = BotConfiguration.ConsoleLogger()
            };
            new Client(configuration).Run();
        }
    }

    [StaticCommand]
    public class EchoCommand : ICommand
    {
        private readonly Message message;

        public bool Suitable => message.Text == "hello";

        public Response Execute()
        {
            return new Response().AddMessage(new TextMessage(message.Chat.Id, message.Text));
        }

        public EchoCommand(Message message) => this.message = message;
    }
}