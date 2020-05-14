using System;
using System.Collections.Generic;
using BotFramework.Bot;
using BotFramework.Commands;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Responses
{
    public class Response
    {
        public          bool                   UsePreviousCommands;
        public readonly List<IResponseMessage> Responses = new List<IResponseMessage>();

        private readonly List<Type>                nextPossible = new List<Type>();
        public           IReadOnlyCollection<Type> NextPossible => nextPossible;

        public Response UsePrevious()
        {
            UsePreviousCommands = true;
            return this;
        }

        public Response AddPossible<T>() where T : ICommand
        {
            nextPossible.Add(typeof(T));
            return this;
        }

        public Response AddPossible<T, T1>() where T : ICommand where T1 : ICommand
        {
            nextPossible.Add(typeof(T));
            nextPossible.Add(typeof(T1));
            return this;
        }

        public Response AddPossible<T, T1, T2>() where T : ICommand where T1 : ICommand where T2 : ICommand
        {
            nextPossible.Add(typeof(T));
            nextPossible.Add(typeof(T1));
            nextPossible.Add(typeof(T2));
            return this;
        }

        public Response AddPossible<T, T1, T2, T3>() where T : ICommand where T1 : ICommand where T2 : ICommand where T3 : ICommand
        {
            nextPossible.Add(typeof(T));
            nextPossible.Add(typeof(T1));
            nextPossible.Add(typeof(T2));
            nextPossible.Add(typeof(T3));
            return this;
        }

        public Response AddPossible<T, T1, T2, T3, T4>() where T : ICommand where T1 : ICommand where T2 : ICommand where T3 : ICommand where T4 : ICommand
        {
            nextPossible.Add(typeof(T));
            nextPossible.Add(typeof(T1));
            nextPossible.Add(typeof(T2));
            nextPossible.Add(typeof(T3));
            nextPossible.Add(typeof(T4));
            return this;
        }

        public Response AddPossible<T, T1, T2, T3, T4, T5>() where T : ICommand where T1 : ICommand where T2 : ICommand where T3 : ICommand where T4 : ICommand where T5 : ICommand
        {
            nextPossible.Add(typeof(T));
            nextPossible.Add(typeof(T1));
            nextPossible.Add(typeof(T2));
            nextPossible.Add(typeof(T3));
            nextPossible.Add(typeof(T4));
            nextPossible.Add(typeof(T5));
            return this;
        }

        public Response AddPossible(params Type[] commands)
        {
            nextPossible.AddRange(BotConfiguration.CheckICommand(commands));
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