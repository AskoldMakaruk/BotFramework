using System.Linq;
using BotFramework.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Commands
{
    public abstract class InputCommand : ICommand
    {
        public abstract MessageType[] InputTypes { get; }

        public Response Execute(Message message, Client client, long accId)
        {
            return !Suitable(message, accId)
                   ? new Response().TextMessage(accId, "BadInput")
                   : Run(message, client, accId);
        }

        public virtual bool Suitable(Message message, long accId)
        {
            return InputTypes.Contains(message.Type);
        }

        protected abstract Response Run(Message message, Client client, long accId);
    }
}