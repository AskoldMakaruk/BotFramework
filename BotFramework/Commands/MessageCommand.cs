using BotFramework.Bot;
using BotFramework.Responses;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Commands
{
    public abstract class MessageCommand : ICommand
    {
        public Response Run(Update update, Client client) =>
        Execute(update.Message, client);

        public bool Suitable(Update message) =>
        message.Message != null && message.Type == UpdateType.Message && Suitable(message.Message);

        public abstract bool Suitable(Message message);

        public abstract Response Execute(Message message, Client client);
    }
}