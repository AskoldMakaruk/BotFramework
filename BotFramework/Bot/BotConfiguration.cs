using System.Collections.Generic;
using System.Reflection;
using BotFramework.Commands;
using Serilog;

namespace BotFramework.Bot
{
    public class BotConfiguration
    {
        public bool                  Webhook  { get; set; }
        public string                Token    { get; set; }
        public string                Name     { get; set; }
        public Assembly              Assembly { get; set; }
        public ILogger               Logger   { get; set; }
        public IEnumerable<ICommand> Commands { get; set; }
    }
}