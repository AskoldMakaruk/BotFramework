using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Responses
{
    public class EditTextMessage : ResponseMessage<Message>
    {
        public EditTextMessage(ChatId               chatId,
                              int                  messageId,
                              string               text,
                              ParseMode            parseMode             = ParseMode.Default,
                              bool                 disableWebPagePreview = false,
                              InlineKeyboardMarkup replyMarkup           = null)
        {
            ChatId                = chatId;
            MessageId             = messageId;
            Text                  = text;
            ParseMode             = parseMode;
            DisableWebPagePreview = disableWebPagePreview;
            ReplyMarkup           = replyMarkup;
        }

        public ChatId               ChatId                { get; }
        public int                  MessageId             { get; }
        public string               Text                  { get; }
        public ParseMode            ParseMode             { get; }
        public bool                 DisableWebPagePreview { get; }
        public InlineKeyboardMarkup ReplyMarkup           { get; }

        protected override Task<Message> Send(TelegramBotClient botClient) =>
        botClient.EditMessageTextAsync(ChatId, MessageId, Text, ParseMode, DisableWebPagePreview, ReplyMarkup);
    }
}