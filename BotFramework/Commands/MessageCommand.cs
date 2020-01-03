using BotFramework.Bot;
using BotFramework.Responses;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Commands
{
    public abstract class MessageCommand : ICommand
    {
        public          Response   Execute(Update  update,  Client client) => Execute(update.Message, client);
        public abstract Response   Execute(Message message, Client client);
        public          UpdateType UpdateType => UpdateType.Message;
    }
}