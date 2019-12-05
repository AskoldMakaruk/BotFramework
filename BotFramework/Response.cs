using System.Collections.Generic;
using System.Collections.Immutable;
using BotFramework.Commands;
using Monads;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework
{
    public class Response
    {
        public Response(ICommand command)
        {
            NextPossible = new Either<ICommand, IEnumerable<IOneOfMany>>(command);
            Responses = ImmutableList<ResponseMessage>.Empty;
        }

        public Response(params IOneOfMany[] nextPossible)
        {
            if(nextPossible.Length > 0)
                NextPossible = new Optional<Either<ICommand, IEnumerable<IOneOfMany>>>(nextPossible);
            Responses = ImmutableList<ResponseMessage>.Empty;
        }

        public Response(IEnumerable<IOneOfMany> nextPossible)
        {
            NextPossible = new Either<ICommand, IEnumerable<IOneOfMany>>(nextPossible);
            Responses = ImmutableList<ResponseMessage>.Empty;
        }

        private Response(Response old, ImmutableList<ResponseMessage> newResponses)
        {
           Responses = newResponses;
           NextPossible = old.NextPossible;
        }

        public readonly ImmutableList<ResponseMessage> Responses; 

        public readonly Optional<Either<ICommand, IEnumerable<IOneOfMany>>> NextPossible;

#region Constructors

        public Response SendTextMessage(ChatId chat,                 string    text, IReplyMarkup replyMarkup = null,
                                        int    replyToMessageId = 0, ParseMode parseMode = default)
        {
            var toAdd = new ResponseMessage(ResponseType.TextMessage)
            {
                ChatId           = chat,
                Text             = text,
                ReplyMarkup      = replyMarkup,
                ReplyToMessageId = replyToMessageId,
                ParseMode        = parseMode
            };
            return new Response(this, this.Responses.Add(toAdd));
        }

        public Response EditTextMessage(ChatId       chatId,             int       editMessageId, string text,
                                        IReplyMarkup replyMarkup = null, ParseMode parseMode = default)
        {
            var toAdd = new ResponseMessage(ResponseType.EditTextMesage)
            {
                ChatId        = chatId,
                EditMessageId = editMessageId,
                Text          = text,
                ReplyMarkup   = replyMarkup,
                ParseMode     = parseMode
            };
            return new Response(this, this.Responses.Add(toAdd));
        }

        public Response AnswerQueryMessage(string callbackQueryId, string text)
        {
            var toAdd = new ResponseMessage(ResponseType.AnswerQuery)
            {
                AnswerToMessageId = callbackQueryId,
                Text              = text
            };
            return new Response(this, this.Responses.Add(toAdd));
        }

        public Response SendDocument(long            account,
                                     InputOnlineFile document,
                                     string          caption          = null,
                                     int             replyToMessageId = 0,
                                     IReplyMarkup    replyMarkup      = null)
        {
            var toAdd = new ResponseMessage(ResponseType.SendDocument)
            {
                ChatId           = account,
                Text             = caption,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup      = replyMarkup,
                Document         = document
            };
            return new Response(this, this.Responses.Add(toAdd));
        }

        public Response EditMessageMarkup(ChatId               accountChatId, int messageMessageId,
                                          InlineKeyboardMarkup addMemeButton)
        {
            var toAdd = new ResponseMessage(ResponseType.EditMessageMarkup) {ChatId = accountChatId, MessageId = messageMessageId, ReplyMarkup = addMemeButton};
            return new Response(this, this.Responses.Add(toAdd));
        }

        public Response SendPhoto(ChatId accountChatId,        InputOnlineFile document, string caption = null,
                                  int    replyToMessageId = 0, IReplyMarkup    replyMarkup = null)
        {
            var toAdd = new ResponseMessage(ResponseType.SendPhoto)
            {
                ChatId           = accountChatId,
                Text             = caption,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup      = replyMarkup,
                Document         = document
            };
            return new Response(this, this.Responses.Add(toAdd));
        }

        public Response SendSticker(ChatId accountChatId, InputOnlineFile sticker)
        {
            var toAdd = new ResponseMessage(ResponseType.Sticker)
            {
                ChatId   = accountChatId,
                Document = sticker
            };
            return new Response(this, this.Responses.Add(toAdd));
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
        EditMessageMarkup,
        Sticker
    }
}
