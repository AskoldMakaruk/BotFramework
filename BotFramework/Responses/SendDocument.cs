using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Responses
{
    public class SendDocument : IResponseMessage
    {
        public SendDocument(ChatId          chatId,
                            InputOnlineFile document,
                            string          caption             = null,
                            ParseMode       parseMode           = ParseMode.Default,
                            bool            disableNotification = false,
                            int             replyToMessageId    = 0,
                            IReplyMarkup    replyMarkup         = null,
                            InputMedia      thumb               = null)
        {
            ChatId              = chatId;
            Document            = document;
            Caption             = caption;
            ParseMode           = parseMode;
            DisableNotification = disableNotification;
            ReplyToMessageId    = replyToMessageId;
            ReplyMarkup         = replyMarkup;
            Thumb               = thumb;
        }

        public ChatId          ChatId              { get; }
        public InputOnlineFile Document            { get; }
        public string          Caption             { get; }
        public ParseMode       ParseMode           { get; }
        public bool            DisableNotification { get; }
        public int             ReplyToMessageId    { get; }
        public IReplyMarkup    ReplyMarkup         { get; }
        public InputMedia      Thumb               { get; }

        public async Task Send(TelegramBotClient botClient)
        {
            await botClient.SendDocumentAsync(ChatId, Document, Caption, ParseMode, DisableNotification, ReplyToMessageId,
                ReplyMarkup, thumb: Thumb);
        }
    }
}