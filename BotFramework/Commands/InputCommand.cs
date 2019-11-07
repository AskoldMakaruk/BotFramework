using System.Linq;
using BotFramework.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Commands
{
    public abstract class InputCommand : ICommand
    {
        public abstract MessageType[] InputTypes { get; }

        public Response Execute(Message message, Client client)
        {
            return !Suitable(message)
                   ? new Response().TextMessage(message.Chat.Id, "BadInput")
                   : Run(message, client);
        }

        public virtual bool Suitable(Message message)
        {
            return InputTypes.Contains(message.Type);
        }

        protected abstract Response Run(Message message, Client client);
    }
}