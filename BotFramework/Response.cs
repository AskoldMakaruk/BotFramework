using System.Collections.Generic;
using BotFramework.Commands;
using Monad;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework
{
    /*
      todo add command to all constructors
      todo set dafalt next commands as prev commands
     */
    public class Response
    {
        public Response(ICommand command)
        {
            NextPossible = EitherStrict.Left<ICommand, IEnumerable<IOneOfMany>>(command);
        }

        public Response(params IOneOfMany[] nextPossible)
        {
            NextPossible = nextPossible.Length == 0 ? null : nextPossible;
        }

        public Response(IEnumerable<IOneOfMany> nextPossible)
        {
            NextPossible = EitherStrict.Right<ICommand, IEnumerable<IOneOfMany>>(nextPossible);
        }

        public List<ResponseMessage> Responses { get; set; } = new List<ResponseMessage>();

        public EitherStrict<ICommand, IEnumerable<IOneOfMany>>? NextPossible { get; }

#region Constructors

        public Response SendTextMessage(ChatId chat,                 string    text, IReplyMarkup replyMarkup = null,
                                        int    replyToMessageId = 0, ParseMode parseMode = default)
        {
            Responses.Add(new ResponseMessage(ResponseType.TextMessage)
            {
                ChatId           = chat,
                Text             = text,
                ReplyMarkup      = replyMarkup,
                ReplyToMessageId = replyToMessageId,
                ParseMode        = parseMode
            });
            return this;
        }

        public Response EditTextMessage(ChatId       chatId,             int       editMessageId, string text,
                                        IReplyMarkup replyMarkup = null, ParseMode parseMode = default)
        {
            Responses.Add(new ResponseMessage(ResponseType.EditTextMesage)
            {
                ChatId        = chatId,
                EditMessageId = editMessageId,
                Text          = text,
                ReplyMarkup   = replyMarkup,
                ParseMode     = parseMode
            });
            return this;
        }

        public Response AnswerQueryMessage(string answerToMessageId, string text)
        {
            Responses.Add(new ResponseMessage(ResponseType.AnswerQuery)
            {
                AnswerToMessageId = answerToMessageId,
                Text              = text
            });
            return this;
        }

        public Response SendDocument(long            account,
                                     InputOnlineFile document,
                                     string          caption          = null,
                                     int             replyToMessageId = 0,
                                     IReplyMarkup    replyMarkup      = null)
        {
            Responses.Add(new ResponseMessage(ResponseType.SendDocument)
            {
                ChatId           = account,
                Text             = caption,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup      = replyMarkup,
                Document         = document
            });
            return this;
        }

        public Response EditMessageMarkup(ChatId               accountChatId, int messageMessageId,
                                          InlineKeyboardMarkup addMemeButton)
        {
            Responses.Add(new ResponseMessage(ResponseType.EditMessageMarkup)
            {ChatId = accountChatId, MessageId = messageMessageId, ReplyMarkup = addMemeButton});
            return this;
        }

        public Response SendPhoto(ChatId accountChatId,        InputOnlineFile document, string caption = null,
                                  int    replyToMessageId = 0, IReplyMarkup    replyMarkup = null)
        {
            Responses.Add(new ResponseMessage(ResponseType.SendPhoto)
            {
                ChatId           = accountChatId,
                Text             = caption,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup      = replyMarkup,
                Document         = document
            });
            return this;
        }

#endregion
    }

    public class ResponseMessage
    {
        public ResponseMessage(ResponseType type)
        {
            Type = type;
        }

        public ResponseMessage() { }

        public ChatId                        ChatId            { get; set; }
        public string                        Text              { get; set; }
        public int                           ReplyToMessageId  { get; set; }
        public IReplyMarkup                  ReplyMarkup       { get; set; }
        public ParseMode                     ParseMode         { get; set; }
        public int                           EditMessageId     { get; set; }
        public string                        AnswerToMessageId { get; set; }
        public InputOnlineFile               Document          { get; set; }
        public ResponseType                  Type              { get; }
        public IEnumerable<IAlbumInputMedia> Album             { get; set; }
        public int                           MessageId         { get; set; }
    }

    public enum ResponseType
    {
        TextMessage,
        EditTextMesage,
        AnswerQuery,
        SendDocument,
        SendPhoto,
        Album,
        EditMessageMarkup
    }
}