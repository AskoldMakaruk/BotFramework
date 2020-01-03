using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Responses
{
    public class SendPhoto : IResponseMessage
    {
        public ResponseType Type => ResponseType.SendPhoto;

        public SendPhoto(ChatId          chatId,
                         InputOnlineFile photo,
                         string          caption             = null,
                         ParseMode       parseMode           = ParseMode.Default,
                         bool            disableNotification = false,
                         int             replyToMessageId    = 0,
                         IReplyMarkup    replyMarkup         = null)
        {
            ChatId              = chatId;
            Photo               = photo;
            Caption             = caption;
            ParseMode           = parseMode;
            DisableNotification = disableNotification;
            ReplyToMessageId    = replyToMessageId;
            ReplyMarkup         = replyMarkup;
        }

        public ChatId          ChatId              { get; }
        public InputOnlineFile Photo               { get; }
        public string          Caption             { get; }
        public ParseMode       ParseMode           { get; }
        public bool            DisableNotification { get; }
        public int             ReplyToMessageId    { get; }
        public IReplyMarkup    ReplyMarkup         { get; }
    }
}