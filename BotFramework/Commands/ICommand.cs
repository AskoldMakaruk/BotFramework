using BotFramework.Bot;
using BotFramework.Responses;
using Monads;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Commands
{
    public interface ICommand
    {
        Optional<Response> Run(Update message, Client client);
    }

    public interface IStaticCommand : ICommand { }
}