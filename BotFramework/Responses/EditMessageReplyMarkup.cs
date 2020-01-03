using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Responses
{
    public class EditMessageReplyMarkup : IResponseMessage
    {
        public ResponseType Type => ResponseType.EditMessageMarkup;

        public EditMessageReplyMarkup(ChatId               chatId,
                                      int                  messageId,
                                      InlineKeyboardMarkup replyMarkup = null)
        {
            ChatId      = chatId;
            MessageId   = messageId;
            ReplyMarkup = replyMarkup;
        }

        public ChatId               ChatId      { get; }
        public int                  MessageId   { get; }
        public InlineKeyboardMarkup ReplyMarkup { get; }
    }
}