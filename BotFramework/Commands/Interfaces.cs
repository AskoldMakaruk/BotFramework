using BotFramework.Bot;
using Telegram.Bot.Types;

namespace BotFramework.Commands
{
    public interface ICommand
    {
        Response Execute(Message message, Client client);
    }

    public interface IOneOfMany : ICommand
    {
        bool Suitable(Message message);
    }
}