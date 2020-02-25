using System.Collections.Generic;
using System.Reflection;
using BotFramework.Commands;
using Monad;
using Serilog;

namespace BotFramework.Bot
{
    public class BotConfiguration
    {
        public bool                  Webhook  { get; set; }
        public string                Token    { get; set; }
        public string                Name     { get; set; }
        public ILogger               Logger   { get; set; }
        public List<ICommand> Commands { get; set; }
        public Optional<ICommand> OnStartCommand { get; set; }
    }
}