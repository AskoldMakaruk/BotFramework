using BotFramework.Bot;
using Optional;
using Telegram.Bot.Types;

namespace BotFramework.Commands.Validators
{
    public class MessageValidator : Validator<Message>
    {
        public Option<Message> Validate(Update update, IGetOnlyClient client) => update.Message.SomeNotNull();
    }
}