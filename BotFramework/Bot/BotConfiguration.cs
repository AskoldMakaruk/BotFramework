using System.Reflection;
using Serilog;

namespace BotFramework.Bot
{
    public class BotConfiguration
    {
        public bool     Webhook  { get; set; }
        public string   Token    { get; set; }
        public string   Name     { get; set; }
        public Assembly Assembly { get; set; }
        public ILogger   Logger   { get; set; }
    }
}