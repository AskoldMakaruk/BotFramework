using System;
using Optional;
using ValueOf;
using Telegram.Bot.Types;

namespace BotFramework.Commands.Validators
{
    public class MessageWithTextValidator : Validator<MessageWithText>
    {
        private readonly Message? message;

        public Option<MessageWithText> Validate()
        {
            return message?.Text != null ? MessageWithText.From(message).Some() : Option.None<MessageWithText>();
        }

        public MessageWithTextValidator(Update update) => message = update.Message;
    }

    public class MessageWithText : ValueOf<Message, MessageWithText>
    {
        protected override void Validate()
        {
            if (Value.Text == null)
                throw new NullReferenceException("Text is null");
        }
    }
}