using Telegram.Bot.Types;

namespace BotFramework.Commands
{
    public abstract class StaticMessageCommand : MessageCommand, IStaticCommand
    {
        public abstract bool Suitable(Message message);
        public          bool Suitable(Update  update) => update.Message != null && Suitable(update.Message);
    }
}