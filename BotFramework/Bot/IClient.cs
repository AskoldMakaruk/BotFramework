using System.Reflection;

namespace BotFramework.Bot
{
    public delegate void Log(IClient sender, string value);

    public interface IClient
    {
        string Name { get; }

        ClientStatus Status { get; set; }

        void Configure(Configuration configuration);

        void HandleUpdate(string json);

        event Log OnLog;
    }

    public enum ClientStatus
    {
        BrokenConfig,
        Broken,
        Stoped,
        Running,
    }

    public class Configuration
    {
        public bool     Webhook  { get; set; }
        public string   Token    { get; set; }
        public string   Name     { get; set; }
        public Assembly Assembly { get; set; }
    }
}