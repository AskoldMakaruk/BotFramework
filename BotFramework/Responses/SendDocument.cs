using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Responses
{
    public class SendDocument : IResponseMessage
    {
        public ResponseType Type => ResponseType.SendDocument;

        public SendDocument(long            account,
                            InputOnlineFile document,
                            string          caption          = null,
                            int             replyToMessageId = 0,
                            IReplyMarkup    replyMarkup      = null)
        {
            Account          = account;
            Document         = document;
            Caption          = caption;
            ReplyToMessageId = replyToMessageId;
            ReplyMarkup      = replyMarkup;
        }

        public long            Account          { get; }
        public InputOnlineFile Document         { get; }
        public string          Caption          { get; }
        public int             ReplyToMessageId { get; }
        public IReplyMarkup    ReplyMarkup      { get; }
    }
}