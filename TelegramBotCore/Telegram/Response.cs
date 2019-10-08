using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotCore.DB.Model;

namespace TelegramBotCore.Telegram
{
    public class Response
    {
        public static Response TextMessage(Account account, string text, IReplyMarkup replyMarkup = null,
                                           int     replyToMessageId = 0)
            => new Response(account, text, replyMarkup, replyToMessageId);

        private Response(Account account, string text, IReplyMarkup replyMarkup = null, int replyToMessageId = 0)
        {
            Account          = account;
            Text             = text;
            ReplyToMessageId = replyToMessageId;
            ReplyMarkup      = replyMarkup;
            Type             = ResponseType.TextMessage;
        }

        public static Response EditTextMessage(Account      account, int editMessageId, string text,
                                               IReplyMarkup replyMarkup = null)
            => new Response(account, editMessageId, text, replyMarkup);

        private Response(Account account, int editMessageId, string text, IReplyMarkup replyMarkup = null)
        {
            Account       = account;
            Text          = text;
            ReplyMarkup   = replyMarkup;
            EditMessageId = editMessageId;
            Type          = ResponseType.EditTextMesage;
        }

        public static Response AnswerQueryMessage(string answerToMessageId, string text)
            => new Response(answerToMessageId, text, true);

        private Response(string answerToMessageId, string text, bool answerQuery = true)
        {
            AnswerToMessageId = answerToMessageId;
            Text              = text;
            AnswerQuery       = answerQuery;
            Type              = ResponseType.AnswerQuery;
        }

        public static Response SendDocument(Account         account,
                                            InputOnlineFile document,
                                            string          caption          = null,
                                            int             replyToMessageId = 0,
                                            IReplyMarkup    replyMarkup      = null)
            => new Response(account, document, caption, replyToMessageId, replyMarkup);

        private Response(Account         account,
                         InputOnlineFile document,
                         string          caption = null,
                         //ParseMode         parseMode           = ParseMode.Default,
                         //bool              disableNotification = false,
                         int          replyToMessageId = 0,
                         IReplyMarkup replyMarkup      = null
            //CancellationToken cancellationToken   = default(CancellationToken),
            //InputMedia        thumb               = null
        )
        {
            Account          = account;
            Text             = caption;
            ReplyToMessageId = replyToMessageId;
            ReplyMarkup      = replyMarkup;
            Document         = document;
            Type             = ResponseType.SendDocument;
        }


        public Account         Account           { get; set; }
        public string          Text              { get; set; }
        public int             ReplyToMessageId  { get; set; } = 0;
        public IReplyMarkup    ReplyMarkup       { get; set; }
        public int             EditMessageId     { get; set; } = 0;
        public bool            AnswerQuery       { get; set; } = false;
        public string          AnswerToMessageId { get; set; }
        public InputOnlineFile Document          { get; set; }
        public ResponseType    Type              { get; private set; }
    }

    public enum ResponseType
    {
        TextMessage,
        EditTextMesage,
        AnswerQuery,
        SendDocument
    }
}