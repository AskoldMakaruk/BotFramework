using BotFramework.Bot;
using BotFramework.Responses;
using Monads;
using Telegram.Bot.Types;

namespace BotFramework.Commands
{
    public interface ICommand
    {
        Response Run(Update message, Client client);
        bool Suitable(Update message);
    }

    public interface IStaticCommand : ICommand { }
}