using Telegram.Bot.Types;

namespace BotFramework.Storage
{
    public interface IUpdateHandler
    {
        void Handle(Update update);
    }
}