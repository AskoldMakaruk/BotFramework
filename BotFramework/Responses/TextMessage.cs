using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Responses
{
    public class TextMessage : ResponseMessage<Message>
    {
        public TextMessage(ChatId       chatId,
                           string       text,
                           ParseMode    parseMode             = ParseMode.Default,
                           bool         disableWebPagePreview = false,
                           bool         disableNotification   = false,
                           int          replyToMessageId      = 0,
                           IReplyMarkup replyMarkup           = null)
        {
            ChatId                = chatId;
            Text                  = text;
            ReplyMarkup           = replyMarkup;
            ReplyToMessageId      = replyToMessageId;
            ParseMode             = parseMode;
            DisableWebPagePreview = disableWebPagePreview;
            DisableNotification   = disableNotification;
        }

        public ChatId       ChatId                { get; set; }
        public string       Text                  { get; set; }
        public int          ReplyToMessageId      { get; set; }
        public ParseMode    ParseMode             { get; set; }
        public bool         DisableWebPagePreview { get; }
        public bool         DisableNotification   { get; }
        public IReplyMarkup ReplyMarkup           { get; set; }

        protected override Task<Message> Send(TelegramBotClient botClient) =>
        botClient.SendTextMessageAsync(ChatId, Text, ParseMode, DisableWebPagePreview, DisableNotification,
            ReplyToMessageId, ReplyMarkup);
    }
}