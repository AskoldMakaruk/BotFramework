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
        public IReadOnlyList<(StaticCommand, Type)> StaticCommands  = null!;
        public IInjector                            CommandInjector = null!;
        public ILogger                              Logger          = null!;
        public TelegramBotClient                    BotClient       = null!;
    }
}