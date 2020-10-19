using System;
using System.Collections.Generic;
using BotFramework.Commands;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Responses
{
    public static class Responses
    {
        public static Response Ok() => new Response();

        public static void NextCommand<T>() where T : ICommand
        {
            
        }

        public static void NextCommand(Type commandType)
        {
            
        }

        public static Response NextCommand(ICommand command) => new Response(nextCommand: command);
    }

    public readonly struct Response
    {
        public readonly ICommand? NextCommand;
        public Response(ICommand? nextCommand = null)
        {
            NextCommand     = nextCommand;
        }
    }
}