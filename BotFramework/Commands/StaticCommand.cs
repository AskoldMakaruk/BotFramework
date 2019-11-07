using BotFramework.Bot;
using Telegram.Bot.Types;

namespace BotFramework.Commands
{
    public abstract class StaticCommand : IOneOfMany
    {
        public abstract string Alias { get; }

        public virtual bool Suitable(Message message)
        {
            return message.Text == Alias;
        }

        public abstract Response Execute(Message message, Client client);
    }
}