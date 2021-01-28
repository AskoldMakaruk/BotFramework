using System;
using System.Collections.Generic;
using BotFramework.Commands;
using BotFramework.Injectors;
using Serilog;
using Telegram.Bot;

namespace BotFramework.Handlers
{
    //todo record
    public record HandlerConfiguration
    {
        public IReadOnlyList<(IStaticCommand, Type)> StaticCommands;
        public IInjector                             CommandInjector;
        public ILogger                               Logger;
        public TelegramBotClient                     BotClient;
    }
}