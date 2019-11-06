using System.Linq;
using BotFramework.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Commands {
    public abstract class InputCommand : ICommand
    {
        public abstract MessageType[] InputTypes { get; }

        public virtual bool Suitable(Message message, long accId) =>
        InputTypes.Contains(message.Type);

        public Response Execute(Message message, Client client, long accId)
        {
            return !Suitable(message, accId)
                   ? new Response().TextMessage(accId, "BadInput")
                   : Run(message, client, accId);
        }

        protected abstract Response Run(Message message, Client client, long accId);
    }
}