using BotFramework.Bot;
using BotFramework.Responses;
using Monads;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Commands
{
    public abstract class MessageCommand : ICommand
    {
        public Response Run(Update update, Client client) =>
        Execute(update.Message, client);

        public bool Suitable(Update message) =>
            message.Type == UpdateType && Suitable(message.Message);

        public abstract bool Suitable(Message message);

        public abstract Response Execute(Message message, Client client);
        public abstract UpdateType         UpdateType { get; }
    }
}