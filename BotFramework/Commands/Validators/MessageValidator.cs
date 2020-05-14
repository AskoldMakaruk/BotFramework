using Optional;
using Telegram.Bot.Types;

namespace BotFramework.Commands.Validators
{
    public class MessageValidator : Validator<Message>
    {
        private readonly Update update;
        public Option<Message> Validate() => update.Message.SomeNotNull();
        public MessageValidator(Update update) => this.update = update;
    }
}