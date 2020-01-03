using BotFramework.Bot;
using BotFramework.Responses;
using Monads;
using Telegram.Bot.Types;

namespace BotFramework.Commands
{
    public interface ICommand
    {
        Optional<Response> Run(Update message, Client client);
    }

    public interface IStaticCommand : ICommand { }
}