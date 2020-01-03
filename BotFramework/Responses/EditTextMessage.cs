using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Responses
{
    public class EditTextMessage : IResponseMessage
    {
        public EditTextMessage(ChatId       chatId,
                               int          editMessageId,
                               string       text,
                               IReplyMarkup replyMarkup = null,
                               ParseMode    parseMode   = default)
        {
            ChatId        = chatId;
            EditMessageId = editMessageId;
            Text          = text;
            ReplyMarkup   = replyMarkup;
            ParseMode     = parseMode;
        }

        public ChatId       ChatId        { get; set; }
        public string       Text          { get; set; }
        public int          EditMessageId { get; set; }
        public ParseMode    ParseMode     { get; set; }
        public IReplyMarkup ReplyMarkup   { get; set; }

        public async Task Send(TelegramBotClient botClient)
        {
            await botClient.EditMessageTextAsync(ChatId, EditMessageId,
                Text,
                replyMarkup: ReplyMarkup as InlineKeyboardMarkup,
                parseMode: ParseMode);
        }
    }
}