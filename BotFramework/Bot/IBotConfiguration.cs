using System;
using Serilog;
using System.Collections.Generic;
using BotFramework.Commands.Injectors;

namespace BotFramework.Bot
{
    public interface IBotConfiguration
    {
        public bool                      Webhook       { get; }
        public string                    Token         { get; }
        public ILogger                   Logger        { get; }
        public INextCommandStorage       Storage       { get; }
        public IReadOnlyCollection<Type> Commands      { get; }
        public IReadOnlyCollection<Type> StartCommands { get; }
        public DependencyInjector        Injector      { get; }
    }
}