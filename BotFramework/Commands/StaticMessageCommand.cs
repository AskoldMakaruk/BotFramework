using System;
using BotFramework.Bot;
using BotFramework.Responses;
using Monads;
using Telegram.Bot.Types;

namespace BotFramework.Commands
{
    public abstract class StaticMessageCommand : MessageCommand, IStaticCommand
    {
        public abstract bool Suitable(Message message);

        public override Optional<Response> Execute(Message message, Client client) =>
        Suitable(message) ? Execute1(message, client) : new Optional<Response>();

        public abstract Optional<Response> Execute1(Message message, Client client);
    }
}