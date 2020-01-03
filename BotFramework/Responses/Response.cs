using System.Collections.Generic;
using System.Collections.Immutable;
using BotFramework.Commands;
using Monads;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Responses
{
    public class Response
    {
        internal readonly bool UsePreviousCommands;

        public Response UsePrevious(bool usePrevious)
        {
            return new Response(Responses, usePrevious);
        }

        public Response(ICommand command) : this()
        {
            NextPossible = new Either<ICommand, IEnumerable<IOneOfMany>>(command);
        }

        public Response(params IOneOfMany[] nextPossible) : this()
        {
            if (nextPossible.Length > 0)
                NextPossible = new Either<ICommand, IEnumerable<IOneOfMany>>(nextPossible).ToOptional();
        }

        public Response(IEnumerable<IOneOfMany> nextPossible) : this()
        {
            NextPossible = new Either<ICommand, IEnumerable<IOneOfMany>>(nextPossible);
        }

        public Response()
        {
            Responses = ImmutableList<IResponseMessage>.Empty;
        }

        private Response(Optional<Either<ICommand, IEnumerable<IOneOfMany>>> nextPossible)
        {
            NextPossible = nextPossible;
            Responses    = ImmutableList<IResponseMessage>.Empty;
        }

        private Response(Optional<Either<ICommand, IEnumerable<IOneOfMany>>> nextPossible,
                         ImmutableList<IResponseMessage>                     newResponses)
        {
            Responses    = newResponses;
            NextPossible = nextPossible;
        }

        private Response(ImmutableList<IResponseMessage> newResponses, bool usePrevious)
        {
            Responses           = newResponses;
            UsePreviousCommands = usePrevious;
        }

        public readonly ImmutableList<IResponseMessage> Responses;

        public readonly Optional<Either<ICommand, IEnumerable<IOneOfMany>>> NextPossible;

#region Constructors

        public Response SendTextMessage(ChatId       chatId,
                                        string       text,
                                        ParseMode    parseMode             = ParseMode.Default,
                                        bool         disableWebPagePreview = false,
                                        bool         disableNotification   = false,
                                        int          replyToMessageId      = 0,
                                        IReplyMarkup replyMarkup           = null)
        {
            return new Response(NextPossible,
                Responses.Add(new TextMessage(chatId, text, parseMode, disableWebPagePreview, disableNotification,
                    replyToMessageId, replyMarkup)));
        }

        public Response EditTextMessage(ChatId       chatId,
                                        int          editMessageId,
                                        string       text,
                                        IReplyMarkup replyMarkup = null,
                                        ParseMode    parseMode   = default)
        {
            return new Response(NextPossible,
                Responses.Add(new EditTextMessage(chatId, editMessageId, text, replyMarkup, parseMode)));
        }

        public Response AnswerCallbackQuery(string callbackQueryId,
                                            string text      = null,
                                            bool   showAlert = false,
                                            string url       = null,
                                            int    cacheTime = 0)
        {
            return new Response(NextPossible,
                Responses.Add(new AnswerCallbackQuery(callbackQueryId, text, showAlert, url, cacheTime)));
        }

        public Response AnswerInlineQuery(string                             inlineQueryId,
                                          IEnumerable<InlineQueryResultBase> results,
                                          int?                               cacheTime         = null,
                                          bool                               isPersonal        = false,
                                          string                             nextOffset        = null,
                                          string                             switchPmText      = null,
                                          string                             switchPmParameter = null)
        {
            return new Response(NextPossible,
                Responses.Add(new AnswerInlineQuery(inlineQueryId, results, cacheTime, isPersonal, nextOffset, switchPmText,
                    switchPmParameter)));
        }

        public Response SendDocument(long            account,
                                     InputOnlineFile document,
                                     string          caption          = null,
                                     int             replyToMessageId = 0,
                                     IReplyMarkup    replyMarkup      = null)
        {
            return new Response(NextPossible, Responses.Add(new SendDocument(
                account,
                document,
                caption,
                replyToMessageId,
                replyMarkup
            )));
        }

        public Response EditMessageMarkup(ChatId               chatId,
                                          int                  messageId,
                                          InlineKeyboardMarkup replyMarkup = null)
        {
            return new Response(NextPossible, Responses.Add(new EditMessageReplyMarkup(chatId, messageId, replyMarkup)));
        }

        public Response SendPhoto(ChatId          chatId,
                                  InputOnlineFile photo,
                                  string          caption             = null,
                                  ParseMode       parseMode           = ParseMode.Default,
                                  bool            disableNotification = false,
                                  int             replyToMessageId    = 0,
                                  IReplyMarkup    replyMarkup         = null)
        {
            return new Response(NextPossible,
                Responses.Add(
                    new SendPhoto(chatId, photo, caption, parseMode, disableNotification, replyToMessageId, replyMarkup)));
        }

        public Response SendAlbum(IEnumerable<IAlbumInputMedia> inputMedia,
                                  ChatId                        chatId,
                                  bool                          disableNotification = false,
                                  int                           replyToMessageId    = 0)
        {
            return new Response(NextPossible,
                Responses.Add(new SendMediaGroup(inputMedia, chatId, disableNotification, replyToMessageId)));
        }

        public Response SendSticker(ChatId          chatId,
                                    InputOnlineFile sticker,
                                    bool            disableNotification = false,
                                    int             replyToMessageId    = 0,
                                    IReplyMarkup    replyMarkup         = null)
        {
            return new Response(NextPossible,
                Responses.Add(new SendSticker(chatId, sticker, disableNotification, replyToMessageId, replyMarkup)));
        }

#endregion
    }
}