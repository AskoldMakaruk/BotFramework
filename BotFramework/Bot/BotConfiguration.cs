using System;
using System.Reflection;

namespace BotFramework.Bot
{
    public class BotConfiguration
    {
        public string   DataDir  { get; set; }
        public bool     Webhook  { get; set; }
        public string   Token    { get; set; }
        public string   Name     { get; set; }
        public Assembly Assembly { get; set; }
        public Log      OnLog    { get; set; }
    }
}