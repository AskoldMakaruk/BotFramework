using BotFramework.Bot;
using BotFramework.Responses;
using Telegram.Bot.Types;

namespace BotFramework.Commands
{
    public interface ICommand
    {
        Response Run(Update message, Client client);

        bool Suitable(Update message);
    }

    //TODO maybe make [StaticAttibute] instead?
    public interface IStaticCommand : ICommand { }
}