using Optional;
using Telegram.Bot.Types;

namespace BotFramework.Commands.Validators
{
    public class MessageWithTextValidator : Validator<MessageWithText>
    {
        private readonly Message? message;

        public Option<MessageWithText> Validate()
        {
            return message?.Text != null ? new MessageWithText(message).Some() : Option.None<MessageWithText>();
        }

        public MessageWithTextValidator(Update update) => message = update.Message;
    }

    public struct MessageWithText
    {
        public Message Message;
        public string Text => Message.Text;
        public MessageWithText(Message message) => Message = message;
    }
}