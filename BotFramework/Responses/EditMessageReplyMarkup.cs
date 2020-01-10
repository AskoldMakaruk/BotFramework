using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Responses
{
    public class EditMessageReplyMarkup : ResponseMessage<Message>
    {
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

        protected override Task<Message> Send(TelegramBotClient botClient) =>
        botClient.EditMessageReplyMarkupAsync(ChatId, MessageId, ReplyMarkup);
    }
}