using Telegram.Bot.Types;

namespace  TelegramBotCore.Telegram.Commands
{
    public abstract class Command
    {
        public abstract AccountStatus Status { get; }
        public abstract void Execute(Message message, Bot client, Account account);
        public virtual void Relieve(Message message, Bot client, Account account)
        {
            new MainCommand().Execute(message, client, account);
        }
        public virtual bool HasSameStatus(AccountStatus accountStatus)
        {
            return accountStatus == Status;
        }
        public virtual bool ContainsHome(string text, Account a)
        {
            return (text.Contains("На главную") || text.Contains("/done") || text == "/exit");
        }
    }
}
