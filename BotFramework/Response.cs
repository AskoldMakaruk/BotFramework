﻿using System.Collections.Generic;
using BotFramework.Commands;
using Monad;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Immutable;

namespace BotFramework
{
    public class Response
    {
        public Response(ICommand command)
        {
            NextPossible = EitherStrict.Left<ICommand, IEnumerable<IOneOfMany>>(command);
            Responses    = ImmutableList<ResponseMessage>.Empty;
        }

        public Response(params IOneOfMany[] nextPossible)
        {
            NextPossible = nextPossible.Length == 0 ? null : nextPossible;
            Responses    = ImmutableList<ResponseMessage>.Empty;
        }

        public Response(IEnumerable<IOneOfMany> nextPossible)
        {
            NextPossible = EitherStrict.Right<ICommand, IEnumerable<IOneOfMany>>(nextPossible);
            Responses    = ImmutableList<ResponseMessage>.Empty;
        }

        private Response(Response old, ImmutableList<ResponseMessage> newResponses)
        {
            Responses    = newResponses;
            NextPossible = old.NextPossible;
        }

        public ImmutableList<ResponseMessage> Responses { get; }

        public readonly EitherStrict<ICommand, IEnumerable<IOneOfMany>>? NextPossible;

#region Constructors

        public Response SendTextMessage(ChatId chat,                 string    text, IReplyMarkup replyMarkup = null,
                                        int    replyToMessageId = 0, ParseMode parseMode = default)
        {
            var newResponses = Responses.Add(new ResponseMessage(ResponseType.TextMessage)
            {
                ChatId           = chat,
                Text             = text,
                ReplyMarkup      = replyMarkup,
                ReplyToMessageId = replyToMessageId,
                ParseMode        = parseMode
            });
            return new Response(this, newResponses);
        }

        public Response EditTextMessage(ChatId       chatId,             int       editMessageId, string text,
                                        IReplyMarkup replyMarkup = null, ParseMode parseMode = default)
        {
            var newResponses = Responses.Add(new ResponseMessage(ResponseType.EditTextMesage)
            {
                ChatId        = chatId,
                EditMessageId = editMessageId,
                Text          = text,
                ReplyMarkup   = replyMarkup,
                ParseMode     = parseMode
            });
            return new Response(this, newResponses);
        }

        public Response AnswerQueryMessage(string callbackQueryId, string text)
        {
            var newResponses = Responses.Add(new ResponseMessage(ResponseType.AnswerQuery)
            {
                AnswerToMessageId = callbackQueryId,
                Text              = text
            });
            return new Response(this, newResponses);
        }

        public Response SendDocument(long            account,
                                     InputOnlineFile document,
                                     string          caption          = null,
                                     int             replyToMessageId = 0,
                                     IReplyMarkup    replyMarkup      = null)
        {
            var newResponses = Responses.Add(new ResponseMessage(ResponseType.SendDocument)
            {
                ChatId           = account,
                Text             = caption,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup      = replyMarkup,
                Document         = document
            });
            return new Response(this, newResponses);
        }

        public Response EditMessageMarkup(ChatId               accountChatId, int messageMessageId,
                                          InlineKeyboardMarkup addMemeButton)
        {
            var newResponses = Responses.Add(new ResponseMessage(ResponseType.EditMessageMarkup)
            {ChatId = accountChatId, MessageId = messageMessageId, ReplyMarkup = addMemeButton});
            return new Response(this, newResponses);
        }

        public Response SendPhoto(ChatId accountChatId,        InputOnlineFile document, string caption = null,
                                  int    replyToMessageId = 0, IReplyMarkup    replyMarkup = null)
        {
            var newResponses = Responses.Add(new ResponseMessage(ResponseType.SendPhoto)
            {
                ChatId           = accountChatId,
                Text             = caption,
                ReplyToMessageId = replyToMessageId,
                ReplyMarkup      = replyMarkup,
                Document         = document
            });
            return new Response(this, newResponses);
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