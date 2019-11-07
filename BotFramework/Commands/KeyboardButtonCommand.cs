using BotFramework.Bot;
using Telegram.Bot.Types;

namespace BotFramework.Commands
{
    public abstract class KeyboardButtonCommand : IOneOfMany
    {
        public abstract string Name { get; }

        public virtual bool Suitable(Message message)
        {
            return message.Text == Name;
        }

        public abstract Response Execute(Message message, Client client);
    }
}