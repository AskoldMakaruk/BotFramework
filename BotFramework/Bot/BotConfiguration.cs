using BotFramework.Commands;
using Serilog;
using System.Collections.Generic;

namespace BotFramework.Bot
{
    public class BotConfiguration
    {
        public bool                Webhook       { get; set; }
        public string              Token         { get; set; }
        public ILogger             Logger        { get; set; }
        public INextCommandStorage Storage       { get; set; }
        public List<ICommand>      Commands      { get; set; } = new List<ICommand>();
        public List<ICommand>      StartCommands { get; set; } = new List<ICommand>();
    }
}