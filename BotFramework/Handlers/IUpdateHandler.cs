using System;
using BotFramework.Helpers;
using Telegram.Bot.Types;

namespace BotFramework.Handlers
{
    public interface IUpdateHandler
    {
        void Handle(Update update);
    }
}