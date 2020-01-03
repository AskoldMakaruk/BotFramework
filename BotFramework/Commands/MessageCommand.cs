using BotFramework.Bot;
using BotFramework.Responses;
using Monads;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Commands
{
    public abstract class MessageCommand : ICommand
    {
        public Optional<Response> Run(Update update, Client client) =>
        update.Type == UpdateType
        ? Execute(update.Message, client)
        : new Optional<Response>();

        public abstract Optional<Response> Execute(Message message, Client client);
        public abstract UpdateType         UpdateType { get; }
    }
}