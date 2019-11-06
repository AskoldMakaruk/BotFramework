namespace BotFramework.Bot
{
    public delegate void Log(IClient sender, string text);

    public interface IClient
    {
        void      Configure(Configuration configuration);
        void      HandleUpdate(string     json);
        event Log OnLog;
    }

    public class Configuration
    {
        public bool   Webhook { get; set; }
        public string Token   { get; set; }
    }
}