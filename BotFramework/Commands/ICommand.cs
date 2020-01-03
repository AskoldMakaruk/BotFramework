using BotFramework.Bot;
using BotFramework.Responses;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Commands
{
    public interface ICommand
    {
        Response   Execute(Update message, Client client);
        UpdateType UpdateType { get; }
    }

    public interface IOneOfMany : ICommand
    {
        bool     Suitable(Update update);
    }

    public interface IStaticCommand : IOneOfMany { }
}