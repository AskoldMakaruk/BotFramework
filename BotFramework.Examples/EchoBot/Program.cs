using System;
using System.Collections.Generic;
using BotFramework.Bot;
using BotFramework.Commands;
using BotFramework.Commands.Validators;
using BotFramework.Responses;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Optional;
using Optional.Linq;
using Telegram.Bot.Types;

namespace EchoBot
{
    class Program
    {
        static void Main()
        {
            Func<Update, IGetOnlyClient, Option<EchoCommand>> f1 = (update, client) =>
            from u in update.Some()
            from a in u.Some()
            from mesValidator in new MessageValidator(u).Validate()
            from helloValidator in new HelloCommandValidator(mesValidator).Validate()
            select new EchoCommand(helloValidator);
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

        public EchoCommand(HelloMessage message) => this.message = message;
    }

    public class HelloMessage : Message
    {
        public HelloMessage(Message m) => DependencyInjector.CopyAllParams(this, m);
    }

    public class HelloCommandValidator : Validator<HelloMessage>
    {
        private readonly Message message;

        public Option<HelloMessage> Validate() =>
        new HelloMessage(message).SomeWhen(t => message.Text == "hello");

        public HelloCommandValidator(Message message) => this.message = message;
    }
}