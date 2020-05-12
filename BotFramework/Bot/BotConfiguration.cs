using System;
using BotFramework.Commands;
using Serilog;
using System.Collections.Generic;
using BotFramework.Commands.Injectors;

namespace BotFramework.Bot
{
    public class BotConfiguration
    {
        public bool                   Webhook       { get; set; }
        public string                 Token         { get; set; }
        public ILogger                Logger        { get; set; }
        public INextCommandStorage    Storage       { get; set; }
        public List<Type>             Commands      { get; set; } = new List<Type>();
        public List<Type>             StartCommands { get; set; } = new List<Type>();
        public DependencyInjector     Injector { get; set; }
    }
}