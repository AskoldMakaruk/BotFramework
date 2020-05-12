using System;
using BotFramework.Bot;
using BotFramework.Commands;
using BotFramework.Commands.Validators;
using BotFramework.Responses;
using Optional;
using Optional.Linq;
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

        public Response Execute()
        {
            return new Response().AddMessage(new TextMessage(message.Chat.Id, message.Text));
        }

        public EchoCommand(HelloMessage message) => this.message = message.Message;
    }

    public class HelloMessage
    {
        public Message Message;
        public HelloMessage(Message m) => Message = m;
    }

    public class HelloCommandValidator : Validator<HelloMessage>
    {
        private readonly Message message;

        public Option<HelloMessage> Validate() =>
        new HelloMessage(message).SomeWhen(t => message.Text == "hello");

        public HelloCommandValidator(Message message) => this.message = message;
    }
}