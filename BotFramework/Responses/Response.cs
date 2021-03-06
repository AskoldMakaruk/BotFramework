﻿using System.Collections.Generic;
using BotFramework.Commands;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Responses
{
    public class Response
    {
        public          bool                   UsePreviousCommands;
        public readonly List<IResponseMessage> Responses;

        public readonly ICommand[] NextPossible;

        public Response(params ICommand[] nextPossible)
        {
            NextPossible = nextPossible;
            Responses    = new List<IResponseMessage>();
        }

        public Response UsePrevious(bool usePrevious)
        {
            UsePreviousCommands = usePrevious;
            return this;
        }

        public Response AddMessage(params IResponseMessage[] messages)
        {
            Responses.AddRange(messages);
            return this;
        }

        public Response AddTextMessage(ChatId       chatId,
                                       string       text,
                                       ParseMode    parseMode             = ParseMode.Default,
                                       bool         disableWebPagePreview = false,
                                       bool         disableNotification   = false,
                                       int          replyToMessageId      = 0,
                                       IReplyMarkup replyMarkup           = null)
        {
            Responses.Add(new TextMessage(chatId, text, parseMode, disableWebPagePreview, disableNotification, replyToMessageId,
                replyMarkup));
            return this;
        }
    }
}