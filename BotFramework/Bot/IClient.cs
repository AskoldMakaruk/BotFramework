namespace BotFramework.Bot
{
    public delegate void Log(IClient sender, stri      t);

    public interface IClient
    {
        v         Configure(Config     on configuration);
        void
    HandleUpdate(string     json);
        event Log OnLog;
    }

    public class Configuration
    {
        public bool   Webhook { get; set; }
        public string Token   { get; set; }
    }
}