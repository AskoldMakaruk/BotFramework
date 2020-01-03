using BotFramework.Bot;
using BotFramework.Responses;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Commands
{
    public interface ICommand
    {
        public Response   Execute(Update message, Client client);
        public UpdateType UpdateType { get; }
    }

    public interface IOneOfMany : ICommand
    {
        bool     Suitable(Update update);
    }

    public interface IStaticCommand : IOneOfMany { }
}