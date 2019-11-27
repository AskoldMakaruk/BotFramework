using System.Reflection;

namespace BotFramework.Bot
{
    public class Configuration
    {
        public string   DataDir  { get; set; }
        public bool     Webhook  { get; set; }
        public string   Token    { get; set; }
        public string   Name     { get; set; }
    }
}