using Telegram.Bot.Types;

namespace BotFramework.Commands
{
    public interface IOneOfMany : ICommand
    {
        bool Suitable(Message message);
    }
}